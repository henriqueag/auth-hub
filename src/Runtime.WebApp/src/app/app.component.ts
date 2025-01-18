import { AsyncPipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { ToastModule } from "primeng/toast";
import { MainLayoutComponent } from "./modules/layout/main-layout/main-layout.component";
import { AuthorizationService } from "./modules/security/services/authorization.service";

@Component({
    selector: "app-root",
    standalone: true,
    imports: [
        MainLayoutComponent,
        RouterOutlet,
        AsyncPipe,
        ToastModule,
    ],
    templateUrl: "./app.component.html",
})
export class AppComponent {
    authService = inject(AuthorizationService);
}
