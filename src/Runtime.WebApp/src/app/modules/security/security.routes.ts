import { Routes } from "@angular/router";

export const SECURITY_ROUTES: Routes = [
    {
        path: "",
        pathMatch: "full",
        redirectTo: "login"
    },
    {
        path: "login",
        loadComponent: () => import("./components/login/login.component").then((c) => c.LoginComponent)
    },
    {
        path: "forgot-password",
        loadComponent: () => import("./components/forgot-password/forgot-password.component").then(c => c.ForgotPasswordComponent),
    },
    {
        path: "change-password",
        loadComponent: () => import("./components/change-password/change-password.component").then(c => c.ChangePasswordComponent)
    }
];
