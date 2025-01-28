import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { MessageService, ToastMessageOptions } from "primeng/api";
import { catchError, throwError } from "rxjs";

export const errorResponseInterceptor: HttpInterceptorFn = (request, next) => {
    const messageService = inject(MessageService);

    return next(request).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status == 0) {
                messageService.add(createMessage(error.message, error));
            }

            if (error && (isClientErrorResponse(error) || isServerErrorResponse(error))) {
                const { title, type, errors } = error.error;
                messageService.add(createMessage(title, { type, errors}));
            }

            return throwError(() => error);
        })
    );
};

function isClientErrorResponse(error: HttpErrorResponse) {
    return error.status >= 400 && error.status <= 499;
}

function isServerErrorResponse(error: HttpErrorResponse) {
    return error.status >= 500 && error.status <= 599;
}

function createMessage(detail: string, data: any) {
    return {
        severity: "error",
        summary: "Erro",
        detail: detail,
        data: data,
        sticky: true,
    } as ToastMessageOptions;
}