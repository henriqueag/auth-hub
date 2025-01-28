import { AsyncPipe, NgIf } from "@angular/common";
import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { PrimeTemplate } from "primeng/api";
import { Card } from "primeng/card";
import { ProgressSpinner } from 'primeng/progressspinner';
import { Toast } from "primeng/toast";
import { Observable } from "rxjs";
import { MainLayoutComponent } from "./modules/layout/main-layout/main-layout.component";
import { AuthorizationService } from "./modules/security/services/authorization.service";
import { User } from "./modules/users/models/user.model";

@Component({
    selector: "app-root",
    standalone: true,
    imports: [
        MainLayoutComponent,
        RouterOutlet,
        AsyncPipe,
        Toast,
        ProgressSpinner,
        Card,
        PrimeTemplate,
        NgIf
    ],
    templateUrl: "./app.component.html"
})
export class AppComponent {
    authService = inject(AuthorizationService);
    currentUser$: Observable<User>;

    ngOnInit(): void {
        this.currentUser$ = this.authService.getCurrentUser$();
    }
}
