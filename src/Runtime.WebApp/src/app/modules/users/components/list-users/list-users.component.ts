import { AsyncPipe, NgClass, NgStyle } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { Component, inject } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Button } from "primeng/button";
import { IconField } from "primeng/iconfield";
import { InputIcon } from "primeng/inputicon";
import { InputTextModule } from "primeng/inputtext";
import { Skeleton } from "primeng/skeleton";
import { TableModule } from "primeng/table";
import { ToolbarModule } from "primeng/toolbar";
import { BehaviorSubject, debounceTime, distinctUntilChanged, lastValueFrom } from "rxjs";
import { PagedResponse } from "src/app/modules/shared/paged-response.model";
import { environment } from "src/environments/environment";
import { User } from "../../models/user.model";
import { Tooltip } from 'primeng/tooltip';

@Component({
    selector: "app-list-users",
    standalone: true,
    imports: [
        TableModule, //
        IconField,
        InputIcon,
        InputTextModule,
        ToolbarModule,
        Button,
        Skeleton,
        FormsModule,
        AsyncPipe,
        Tooltip
    ],
    templateUrl: "./list-users.component.html",
    styleUrl: "./list-users.component.scss",
})
export class ListUsersComponent {
    private readonly debounceTime = 300;

    private _httpClient = inject(HttpClient);

    query?: string;
    skip: number = 0;
    limit: number = 10;
    columns = [
        { name: "displayName", displayName: "Nome", isSortable: true },
        { name: "username", displayName: "Nome de usuário", isSortable: true },
        { name: "email", displayName: "Email", isSortable: true },
        { name: "active", displayName: "Ativo", isSortable: false },
        { name: "actions", displayName: "Ações", isSortable: false },
    ]
    users$ = new BehaviorSubject<User[]>([]);
    loading$ = new BehaviorSubject<boolean>(false);
    totalRecords$ = new BehaviorSubject<number>(0);
    search$ = new BehaviorSubject<string>("");

    async ngOnInit() {
        await this.load();

        this.search$
            .pipe(debounceTime(this.debounceTime), distinctUntilChanged())
            .subscribe(async () => await this.load());
    }

    onQueryChange(query) {
        this.query = query;
        this.search$.next(query);
    }

    async onLazyLoad(event) {
        this.skip = event.first;
        this.limit = event.rows;
        await this.load();
    }

    onEdit(id) {

    }

    onDelete(id) {

    }

    async load() {
        const url = `${environment.apiUrl}/api/users`;

        const request = this._httpClient.get<PagedResponse<User>>(url, {
            params: {
                skip: this.skip,
                limit: this.limit,
                q: this.query || "",
            },
        });

        this.loading$.next(true);

        return await lastValueFrom(request)
            .then((value) => {
                this.loading$.next(false);
                this.users$.next(value.items);

                if (this.totalRecords$.getValue() != value.totalRecords) {
                    this.totalRecords$.next(value.totalRecords);
                }
            })
            .catch((error) => {
                this.loading$.next(false);
                throw error;
            });
    }
}
