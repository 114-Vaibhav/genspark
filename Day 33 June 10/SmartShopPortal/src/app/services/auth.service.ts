import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {

  currentUser = signal<User | null>(null);

  constructor(private http: HttpClient) {}

  loginApiCall(loginModel: LoginModel) {
    return this.http.post<User>(
      'https://dummyjson.com/auth/login',
      loginModel
    );
  }

  setCurrentUser(user: User) {
    this.currentUser.set(user);
  }

  logout() {
    this.currentUser.set(null);
    sessionStorage.clear();
  }
}