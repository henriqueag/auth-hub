import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorizationService } from '../services/authorization.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
    private _authService = inject(AuthorizationService);

    canActivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this._authService.authorize();
    }
}