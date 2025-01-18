import { AsyncPipe, JsonPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { BehaviorSubject, interval, map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AuthorizationService } from '../security/services/authorization.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [JsonPipe, AsyncPipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
    client = inject(HttpClient);
    authService = inject(AuthorizationService);

    currentUser$: Observable<any>;

    currentDate = new BehaviorSubject<string>(null)
    tokenDate = new BehaviorSubject<string>(null)

    constructor() {
        interval(1)
            .subscribe(() => {
                this.currentDate.next(new Date().toLocaleString());
                this.tokenDate.next(JSON.parse(localStorage.getItem("security-token"))?.['expires_at_date'])
            })
    }

    onClick() {
        const url = `${environment.apiUrl}/api/current-user`
        this.currentUser$ = this.client.get(url, { observe: "response" })
            .pipe(map(response => ({ status: response.status, url: response.url, body: response.body })));
    }

    onSignOut() {
        this.authService.signOut();
    }
}
