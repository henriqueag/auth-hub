import { Component } from "@angular/core";
import { TableModule } from "primeng/table";
import { IconField } from "primeng/iconfield";
import { InputIcon } from "primeng/inputicon";
import { InputTextModule } from "primeng/inputtext";
import { ToolbarModule } from "primeng/toolbar";
import { Button } from "primeng/button";

@Component({
    selector: "app-list-users",
    standalone: true,
    imports: [
        TableModule,
        IconField,
        InputIcon,
        InputTextModule,
        ToolbarModule,
        Button,
    ],
    templateUrl: "./list-users.component.html",
    styleUrl: "./list-users.component.scss",
})
export class ListUsersComponent {
    users = Array.from({ length: 5 }).map((_, i) => ({
        a: "A " + i,
        b: "B " + i,
        c: "C " + i,
        d: "D " + i,
        e: "E " + i,
    }));
}
