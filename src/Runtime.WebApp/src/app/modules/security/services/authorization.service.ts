import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { SecurityTokenStorageService } from './security-token-storage.service';
import { SecurityTokenService } from './security-token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
    private _isLoggedIn$ = new BehaviorSubject<boolean>(false);
    private _loggingIn$ = new BehaviorSubject<boolean>(false);

    private _tokenService = inject(SecurityTokenService);
    private _tokenStorage = inject(SecurityTokenStorageService);
    private _router = inject(Router);

    get isLoggedIn() {
        return this._tokenStorage.getToken()?.isLoggedIn;
    }

    get isLoggedIn$() {
        this._isLoggedIn$.next(this._tokenStorage.getToken()?.isLoggedIn);
        return this._isLoggedIn$.asObservable();
    }

    get loggingIn$() {
        return this._loggingIn$.asObservable();
    }

    async signIn(email: string, password:string) {
        this._loggingIn$.next(true);

        await this._tokenService
            .createToken(email, password)
            .then(() => {
                this._loggingIn$.next(false);
                this._router.navigate([""])
            })
            .catch(error => {
                this._loggingIn$.next(false);
                this._tokenStorage.revokeToken();
                throw error;
            })
    }

    signOut() {
        this._isLoggedIn$.next(false);
        this._tokenStorage.revokeToken();
        this._router.navigate(["account", "login"]);
    }
}
