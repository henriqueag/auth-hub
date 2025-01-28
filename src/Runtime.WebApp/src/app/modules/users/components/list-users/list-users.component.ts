import { AsyncPipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { ConfirmationService, MessageService } from "primeng/api";
import { Button } from "primeng/button";
import { ConfirmDialog } from "primeng/confirmdialog";
import { IconField } from "primeng/iconfield";
import { InputIcon } from "primeng/inputicon";
import { InputText } from "primeng/inputtext";
import { Skeleton } from "primeng/skeleton";
import { TableModule } from "primeng/table";
import { Toolbar } from "primeng/toolbar";
import { Tooltip } from 'primeng/tooltip';
import { BehaviorSubject, debounceTime, distinctUntilChanged, Observable } from "rxjs";
import { User } from "../../models/user.model";
import { UserManagerService } from "../../services/user-manager.service";

@Component({
    selector: "app-list-users",
    standalone: true,
    imports: [
        TableModule, //
        IconField,
        InputIcon,
        InputText,
        Toolbar,
        Button,
        Skeleton,
        FormsModule,
        AsyncPipe,
        Tooltip,
        ConfirmDialog
    ],
    providers: [
        ConfirmationService
    ],
    templateUrl: "./list-users.component.html",
    styleUrl: "./list-users.component.scss",
})
export class ListUsersComponent {
    private readonly debounceTime = 300;

    private _userManagerService = inject(UserManagerService);
    private _confirmationService = inject(ConfirmationService);
    private _messageService = inject(MessageService);
    private _router = inject(Router);

    query?: string;
    skip: number = 0;
    limit: number = 10;
    columns = [
        { name: "displayName", displayName: "Nome", isSortable: true },
        { name: "username", displayName: "Nome de usuário", isSortable: true },
        { name: "email", displayName: "Email", isSortable: true },
        { name: "active", displayName: "Ativo", isSortable: false },
        { name: "actions", displayName: "Ações", isSortable: false, width: "160px" },
    ]
    loading$: Observable<boolean>
    users$: Observable<User[]>
    totalRecords$: Observable<number>
    search$ = new BehaviorSubject<string>("");

    async ngOnInit() {
        this.loading$ = this._userManagerService.isLoading$();
        this.users$ = this._userManagerService.getUsers$();
        this.totalRecords$ = this._userManagerService.getTotalRecords$();

        // await this._userManagerService.loadUsers(this.skip, this.limit, this.query);

        this.search$
            .pipe(debounceTime(this.debounceTime), distinctUntilChanged())
            .subscribe(() => this._userManagerService.loadUsers(this.skip, this.limit, this.query));
    }

    onQueryChange(query) {
        this.query = query;
        this.search$.next(query);
    }

    async onLazyLoad(event) {
        this.skip = event.first;
        this.limit = event.rows;
        this._userManagerService.loadUsers(this.skip, this.limit, this.query);
    }

    onEdit(id) {
        this._router.navigate(["users", id, "edit"]);
    }

    onDelete(user) {
        this._confirmationService.confirm({
            message: `Deseja prosseguir com a exclusão do usuário ${user.displayName}?`,
            header: "Confirmação",
            closable: true,
            closeOnEscape: true,
            icon: "fa-solid fa-trash",
            rejectButtonProps: {
                label: "Cancelar",
                severity: "secondary",
                outlined: true
            },
            acceptButtonProps: {
                label: "Excluir",
                severity: "danger"
            },
            accept: async () => {
                await this._userManagerService.deleteUser(user.id)
                    .then(async () => {
                        this._messageService.add({
                            severity: "success",
                            summary: "Sucesso",
                            detail: `O usuário ${user.displayName} foi excluído com sucesso.`
                        });

                        this._userManagerService.loadUsers(this.skip, this.limit, this.query);
                    });
            }
        })
    }
}
