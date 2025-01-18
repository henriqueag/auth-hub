import { HttpClient } from '@angular/common/http';
import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BehaviorSubject, catchError, lastValueFrom, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../models/user.model';

const BASE_URL = environment.apiUrl;

@Injectable()
export class UserManagerService {
    private _loading = new BehaviorSubject<boolean>(false);

    private _httpClient = inject(HttpClient);
    private _destroyRef = inject(DestroyRef);

    async getCurrentUser() {
        this._loading.next(true);
        const observable = this._httpClient.get<User>(`${BASE_URL}/api/current-user`)
            .pipe(
                catchError((error) => throwError(() => error)),
                takeUntilDestroyed(this._destroyRef),
            )

        return await lastValueFrom(observable);
    }
}
