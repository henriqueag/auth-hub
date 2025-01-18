import { inject } from "@angular/core";
import { Routes } from "@angular/router";
import { AuthGuard } from "./modules/security/guards/auth.guard";

export const routes: Routes = [
    {
        path: "",
        pathMatch: "full",
        redirectTo: "users"
    },
    {
        path: "account",
        loadChildren: () => import("src/app/modules/security/security.routes").then((c) => c.SECURITY_ROUTES),
    },
    {
        path: "users",
        loadChildren: () => import("src/app/modules/users/user.routes").then((c) => c.USER_ROUTES),
        canActivate: [() => inject(AuthGuard).canActivate()]
    },
    {
        path: "profiles",
        loadComponent: () => import("src/app/modules/home/home.component").then((c) => c.HomeComponent),
        canActivate: [() => inject(AuthGuard).canActivate()]
    },
    {
        path: "**",
        redirectTo: "account",
    }
];
