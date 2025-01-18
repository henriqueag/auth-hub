import Aura from "@primeng/themes/aura";

import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { ApplicationConfig, provideZoneChangeDetection } from "@angular/core";
import { provideAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { MessageService } from "primeng/api";
import { providePrimeNG } from "primeng/config";
import { routes } from "./app.routes";
import { tokenInterceptor } from "./modules/security/interceptors/token/token.interceptor";
import { errorResponseInterceptor } from "./modules/shared/error-response.interceptor";

export const appConfig: ApplicationConfig = {
    providers: [
        MessageService,
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideHttpClient(withInterceptors([
            tokenInterceptor,
            errorResponseInterceptor
        ])),
        provideAnimations(),
        providePrimeNG({
            ripple: true,
            theme: {
                preset: Aura,
                options: {
                    darkModeSelector: "none",
                }
            }
        })
    ],
};
