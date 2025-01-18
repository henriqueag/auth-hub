import { Injectable } from "@angular/core";
import { SecurityToken } from "../models/security-token.model";

@Injectable({
    providedIn: "root"
})
export class SecurityTokenStorageService {
    readonly LocalStorageKey = "security-token";

    getToken() {
        const tokenString = localStorage.getItem(this.LocalStorageKey);
        if (!tokenString) return null;

        const token = JSON.parse(tokenString) as SecurityToken;
        return token;
    }

    storeToken(token: SecurityToken) {
        localStorage.setItem(this.LocalStorageKey, JSON.stringify(token));
    }

    revokeToken() {
        localStorage.removeItem(this.LocalStorageKey);
    }
}