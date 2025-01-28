import { HttpClient } from '@angular/common/http';
import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BehaviorSubject, catchError, lastValueFrom, takeUntil, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../models/user.model';
import { PagedResponse } from '../../shared/models/paged-response.model';
import { AuthorizationService } from '../../security/services/authorization.service';

const BASE_URL = environment.apiUrl;

@Injectable({
    providedIn: "root"
})
export class UserManagerService {
    private _loading$ = new BehaviorSubject<boolean>(false);
    private _deleting$ = new BehaviorSubject<boolean>(false);
    private _users$ = new BehaviorSubject<User[]>([]);
    private _totalRecords$ = new BehaviorSubject<number>(0);

    private _httpClient = inject(HttpClient);
    private _destroyRef = inject(DestroyRef);
    private _authService = inject(AuthorizationService);

    isLoading$ = () => this._loading$.asObservable();
    isDeleting$ = () => this._deleting$.asObservable();
    getUsers$ = () => this._users$.asObservable();
    getTotalRecords$ = () => this._totalRecords$.asObservable();

    loadUsers(skip: number, limit: number, query?: string) {
        const url = `${BASE_URL}/api/users`;
        const params = { skip, limit, q: query || "" };

        this._httpClient.get<PagedResponse<User>>(url, { params: params })
            .pipe(
                takeUntil(this._authService.onLogout$()),
                catchError(error => {
                    this._loading$.next(false);
                    return throwError(() => error);
                })
            )
            .subscribe(value => {
                this._loading$.next(false);
                this._users$.next(value.items);

                if (this._totalRecords$.getValue() != value.totalRecords) {
                    this._totalRecords$.next(value.totalRecords);
                }
            });
    }

    async deleteUser(id: string) {
        const request = this._httpClient.delete(`${BASE_URL}/api/users/${id}`)
            .pipe(
                catchError(error => throwError(() => error)),
                takeUntilDestroyed(this._destroyRef)
            );

        return await lastValueFrom(request);
    }
}