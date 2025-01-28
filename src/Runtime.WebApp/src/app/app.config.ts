import Aura from "@primeng/themes/aura";

import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { ApplicationConfig, provideZoneChangeDetection } from "@angular/core";
import { provideAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { ConfirmationService, MessageService } from "primeng/api";
import { providePrimeNG } from "primeng/config";
import { routes } from "./app.routes";
import { tokenInterceptor } from "./modules/security/interceptors/token/token.interceptor";
import { errorResponseInterceptor } from "./modules/shared/interceptors/error-response.interceptor";
import { pendingRequestsInterceptor } from "./modules/shared/interceptors/pending-requests.interceptor";

export const appConfig: ApplicationConfig = {
    providers: [
        MessageService,
        ConfirmationService,
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideHttpClient(withInterceptors([
            tokenInterceptor,
            errorResponseInterceptor,
            pendingRequestsInterceptor
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
