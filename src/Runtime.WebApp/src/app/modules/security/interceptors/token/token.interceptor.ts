import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { SecurityTokenStorageService } from "../../services/security-token-storage.service";

export const tokenInterceptor: HttpInterceptorFn = (request, next) => {
    const token = inject(SecurityTokenStorageService).getToken();

    if (!token) return next(request);

    return next(request.clone({
        headers: request.headers.append("Authorization", `${token.token_type} ${token.access_token}`)
    }));
};
