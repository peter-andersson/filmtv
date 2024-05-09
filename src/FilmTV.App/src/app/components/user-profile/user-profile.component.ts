import { Component } from '@angular/core';
import {AuthService, AuthState} from '@auth0/auth0-angular';
import {AsyncPipe, CommonModule} from "@angular/common";
import {map, tap} from "rxjs";

@Component({
  selector: 'app-user-profile',
  template: `
    <ul *ngIf="auth.user$ | async as user">
      <li>{{ user.name }}</li>
      <li>{{ user.email }}</li>
      <li>{{ JSON.stringify(user) }}</li>
    </ul>
    <button (click)="getToken()">GetToken</button>
  `,
  imports: [
    AsyncPipe, CommonModule
  ],
  standalone: true
})
export class UserProfileComponent {
  constructor(public auth: AuthService) {}

  getToken(){
    console.log("Token");
    this.auth.getAccessTokenSilently({ detailedResponse: true }).subscribe(token => console.log(token.access_token));
  }

  protected readonly JSON = JSON;
}
