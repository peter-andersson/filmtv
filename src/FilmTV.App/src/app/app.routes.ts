import { Routes } from '@angular/router';
import {UserLoginComponent} from "../user/user-login/user-login.component";
import {UserRegisterComponent} from "../user/user-register/user-register.component";

export const routes: Routes = [
  {path: 'login', component: UserLoginComponent},
  {path: 'register', component: UserRegisterComponent},
];
