import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { MenuItem } from "primeng/api";
import { Menu } from "primeng/menu";
import { Menubar } from "primeng/menubar";
import { Observable } from "rxjs";
import { AuthorizationService } from "../../security/services/authorization.service";
import { Button } from "primeng/button";
import { UserManagerService } from "../../users/services/user-manager.service";

@Component({
    selector: "main-layout",
    standalone: true,
    imports: [
        RouterOutlet, //
        Menu,
        Menubar,
        Button,
    ],
    providers: [UserManagerService],
    templateUrl: "./main-layout.component.html",
})
export class MainLayoutComponent {
    private _authService = inject(AuthorizationService);
    private _userManager = inject(UserManagerService);

    menuItems: MenuItem[] = [
        {
            label: "Usuários",
            icon: "fa-solid fa-users-gear",
            routerLink: "users",
        },
        {
            label: "Perfis",
            icon: "fa-regular fa-id-card",
            routerLink: "profiles",
        },
        {
            label: "Grupos",
            icon: "fa-solid fa-users-viewfinder",
            routerLink: "groups",
        },
    ];

    currentUserMenuItems: MenuItem[] = [];

    isLoggedIn$: Observable<boolean>;

    async ngOnInit() {
        this.isLoggedIn$ = this._authService.isLoggedIn$;

        await this._userManager.getCurrentUser().then((user) =>
            this.currentUserMenuItems = [
                {
                    label: user.displayName,
                    items: [
                        {
                            label: "Sair",
                            icon: "fa-solid fa-right-from-bracket",
                            command: () => this._authService.signOut(),
                        },
                    ],
                },
            ]
        );
    }
}
