import { Routes } from "@angular/router";

export const routes: Routes = [
    {
        path: "",
        pathMatch: "full",
        redirectTo: "login"
    },
    {
        path: "",
        loadChildren: () => import("src/app/modules/security/security.routes").then((c) => c.SECURITY_ROUTES),
    },
    {
        path: "**",
        redirectTo: "login"
    }
];
