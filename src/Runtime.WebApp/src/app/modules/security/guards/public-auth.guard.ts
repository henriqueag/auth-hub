import { Injectable, inject } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, map } from "rxjs";
import { AuthorizationService } from "../services/authorization.service";

@Injectable({
    providedIn: "root"
})
export class PublicAuthGuard {
    private _authService = inject(AuthorizationService);
    private _router = inject(Router);

    canActivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this._authService.isLoggedIn$().pipe(
            map(isLoggedIn => {
                if (isLoggedIn) {
                    this._router.navigate([""]);
                    return true;
                }
                return true;
            })
        )
    }
}