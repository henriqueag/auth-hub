import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorizationService } from '../services/authorization.service';
import { SecurityTokenStorageService } from '../services/security-token-storage.service';
import { SecurityTokenService } from './../services/security-token.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
    private _tokenService = inject(SecurityTokenService);
    private _tokenStorage = inject(SecurityTokenStorageService);
    private _authService = inject(AuthorizationService);
    private _router = inject(Router);

    canActivate(): boolean | Observable<boolean> | Promise<boolean> {
        if (this._authService.isLoggedIn) {
            const token = this._tokenStorage.getToken();
            this._tokenService.scheduleRefreshToken(token);
            return true;
        }

        this._router.navigate(["account", "login"]);
        return false;
    }
}