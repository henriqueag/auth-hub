import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { takeUntil } from "rxjs";
import { AuthorizationService } from "../../security/services/authorization.service";

export const pendingRequestsInterceptor: HttpInterceptorFn = (request, next) => {
    return next(request).pipe(takeUntil(inject(AuthorizationService).onLogout$()));
};