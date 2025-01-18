import { Routes } from "@angular/router";

export const USER_ROUTES: Routes = [
    {
        path: "",
        loadComponent: () => import("./components/list-users/list-users.component").then(c => c.ListUsersComponent)
    },
    {
        path: "create",
        loadComponent: () => import("./components/create-user/create-user.component").then(c => c.CreateUserComponent)
    },
    {
        path: ":id/edit",
        loadComponent: () => import("./components/update-user/update-user.component").then(c => c.UpdateUserComponent)
    }
]