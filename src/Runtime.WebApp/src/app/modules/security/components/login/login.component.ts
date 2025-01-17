import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { PrimeTemplate } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { Card } from "primeng/card";
import { FloatLabelModule } from "primeng/floatlabel";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";

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
        FormsModule
    ],
    templateUrl: "./login.component.html"
})
export class LoginComponent {
    constructor(private _router: Router) { }

    email: string
    password: string
    loading: boolean

    navigateTo(...path: string[]) {
        this._router.navigate(path)
    }
}
