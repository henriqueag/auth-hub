import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { MenuItem } from "primeng/api";
import { Button } from "primeng/button";
import { Menu } from "primeng/menu";
import { Menubar } from "primeng/menubar";
import { Observable, take } from "rxjs";
import { AuthorizationService } from "../../security/services/authorization.service";
import { UserManagerService } from "../../users/services/user-manager.service";

@Component({
    selector: "main-layout",
    standalone: true,
    imports: [
        RouterOutlet, //
        Menu,
        Menubar,
        Button
    ],
    providers: [UserManagerService],
    templateUrl: "./main-layout.component.html",
})
export class MainLayoutComponent {
    private _authService = inject(AuthorizationService);

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

    isLoggedIn$: Observable<boolean>;
    currentUserMenuItems: MenuItem[] = [];

    async ngOnInit() {
        this.isLoggedIn$ = this._authService.isLoggedIn$();
        this._authService.getCurrentUser$()
            .pipe(take(2))
            .subscribe(currentUser => {
                if (!currentUser) return;

                this.currentUserMenuItems = [
                    {
                        label: currentUser.displayName,
                        items: [
                            {
                                label: "Sair",
                                icon: "fa-solid fa-right-from-bracket",
                                command: () => this._authService.signOut(),
                            },
                        ],
                    },
                ]
            })
    }
}
