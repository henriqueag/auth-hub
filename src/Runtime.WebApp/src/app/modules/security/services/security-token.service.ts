import { HttpClient, HttpHeaders } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, first, lastValueFrom } from "rxjs";
import { environment } from "src/environments/environment";
import { SecurityToken } from "../models/security-token.model";
import { SecurityTokenStorageService } from "./security-token-storage.service";

const BASE_URL = environment.apiUrl;
const TOKEN_EXPIRATION_BUFFER = 30 * 1000

@Injectable({
    providedIn: "root",
})
export class SecurityTokenService {
    private _loading = new BehaviorSubject<boolean>(false);

    private _httpClient = inject(HttpClient);
    private _storage = inject(SecurityTokenStorageService);

    private _refreshTokenTimeout = null;

    get loading$() {
        return this._loading.asObservable();
    }

    async createToken(email: string, password: string) {
        const { url, body, headers } = this.createTokenRequest(email, password);

        this._loading.next(true);

        const observable = this._httpClient
            .post<SecurityToken>(url, body, { headers: headers })
            .pipe(first());

        return await lastValueFrom(observable)
            .then(token => this.onCreatingTokenSuccessfully(token))
    }

    async scheduleRefreshToken(token: SecurityToken) {
        if (Date.now() > (token.expires_at - TOKEN_EXPIRATION_BUFFER)) {
            await this.refreshToken();
        }

        if (this._refreshTokenTimeout) {
            clearTimeout(this._refreshTokenTimeout);
        }

        const refreshTime = (token.expires_in * 1000) - TOKEN_EXPIRATION_BUFFER;
        this._refreshTokenTimeout = setTimeout(async () => await this.refreshToken(), refreshTime);
    }

    private async refreshToken() {
        const token =  this._storage.getToken();

        if (!token) return;

        const { url, body, headers } = this.createRefreshTokenRequest(token.refresh_token);
        const observable = this._httpClient
            .post<SecurityToken>(url, body, { headers: headers })
            .pipe(first());

        await lastValueFrom(observable)
            .then((token) => this.onCreatingTokenSuccessfully(token))
            .catch(error => this.onCreatingTokenFailure(error));
    }

    private createTokenRequest(email: string, password: string) {
        const url = `${BASE_URL}/api/security/token`;

        const body = new URLSearchParams({
            grant_type: "password",
            email: email,
            password: password,
        });

        const headers = new HttpHeaders({
            "Content-Type": "application/x-www-form-urlencoded",
        });

        return { url, body, headers };
    }

    private createRefreshTokenRequest(refreshToken: string) {
        const url = `${BASE_URL}/api/security/refresh-token`;

        const body = new URLSearchParams({
            grant_type: "refresh_token",
            refresh_token: refreshToken,
        });

        const headers = new HttpHeaders({
            "Content-Type": "application/x-www-form-urlencoded",
        });

        return { url, body, headers };
    }

    private onCreatingTokenSuccessfully(token: SecurityToken) {
        token.expires_at = Date.now() + (token.expires_in * 1000);
        token['expires_at_date'] = new Date(token.expires_at).toLocaleString();
        token.isLoggedIn = true;

        this._storage.storeToken(token);

        this.scheduleRefreshToken(token);
    }

    private onCreatingTokenFailure(error: any) {
        this._storage.revokeToken();
        throw error;
    }
}