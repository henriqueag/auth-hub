import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, catchError, first, map, Subject, throwError } from "rxjs";
import { environment } from "src/environments/environment";
import { User } from "../../users/models/user.model";
import { SecurityTokenStorageService } from "./security-token-storage.service";
import { SecurityTokenService } from "./security-token.service";

@Injectable({
    providedIn: "root",
})
export class AuthorizationService {
    private _isLoggedIn$ = new BehaviorSubject<boolean>(false);
    private _isLoginInProgress$ = new BehaviorSubject<boolean>(false);
    private _isLoadingAfterLogin$ = new BehaviorSubject<boolean>(false);
    private _currentUser$ = new BehaviorSubject<any>(null);
    private _logout$ = new Subject<void>();

    private _tokenService = inject(SecurityTokenService);
    private _tokenStorage = inject(SecurityTokenStorageService);
    private _httpClient = inject(HttpClient);
    private _router = inject(Router);

    isLoggedIn$ = () => {
        this._isLoggedIn$.next(this._tokenStorage.getToken()?.isLoggedIn);
        return this._isLoggedIn$.asObservable();
    };

    isLoginInProgress$ = () => this._isLoginInProgress$.asObservable();

    isLoadingAfterLogin$ = () => this._isLoadingAfterLogin$.asObservable();

    onLogout$ = () => this._logout$.asObservable();

    authorize() {
        return this.isLoggedIn$().pipe(
            map((isLoggedIn) => {
                if (isLoggedIn) {
                    const token = this._tokenStorage.getToken();
                    this._tokenService.scheduleRefreshToken(token);
                    this.getCurrentUser$();
                    return true;
                }

                this._router.navigate(["account", "login"]);
                return false;
            })
        );
    }

    async signIn(email: string, password: string) {
        this._isLoginInProgress$.next(true);

        await this._tokenService
            .createToken(email, password)
            .then(() => {
                this._isLoginInProgress$.next(false);
                this._isLoadingAfterLogin$.next(true);
                this._router.navigate([""]);

                setTimeout(() => this._isLoadingAfterLogin$.next(false), 1500);
            })
            .catch((error) => {
                this._isLoginInProgress$.next(false);
                this._tokenStorage.revokeToken();
                throw error;
            });
    }

    signOut() {
        this._isLoggedIn$.next(false);
        this._tokenStorage.revokeToken();
        this._currentUser$.next(null);
        this._logout$.next();
        this._router.navigate(["account", "login"]);
    }

    getCurrentUser$() {
        const currentUser = this._currentUser$.getValue();
        if (currentUser) {
            return this._currentUser$.asObservable();
        }

        const url = `${environment.apiUrl}/api/current-user`;

        return this._httpClient
            .get<User>(url)
            .pipe(
                first(),
                map((currentUser) => {
                    this._currentUser$.next(currentUser);
                    return this._currentUser$.getValue();
                }),
                catchError((error) => throwError(() => error))
            );
    }
}
