import { AsyncPipe } from "@angular/common";
import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { PrimeTemplate } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { Card } from "primeng/card";
import { FloatLabelModule } from "primeng/floatlabel";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { Observable, Subscription } from "rxjs";
import { AuthorizationService } from "../../services/authorization.service";

@Component({
    selector: "app-login",
    standalone: true,
    imports: [
        Card,
        ButtonModule,
        PrimeTemplate,
        FloatLabelModule,
        PasswordModule,
        InputTextModule,
        FormsModule,
        AsyncPipe,
    ],
    templateUrl: "./login.component.html",
})
export class LoginComponent implements OnInit, OnDestroy {
    private _router = inject(Router);
    private _authService = inject(AuthorizationService);

    email: string = "admin@email.com";
    password: string = "test@123";
    isLoginInProgress$: Observable<boolean>;
    subscriptions = new Subscription();

    ngOnInit(): void {
        this.isLoginInProgress$ = this._authService.isLoginInProgress$();
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    navigateTo(...path: string[]) {
        this._router.navigate(path);
    }

    async onSignIn() {
        await this._authService.signIn(this.email, this.password);
    }
}
