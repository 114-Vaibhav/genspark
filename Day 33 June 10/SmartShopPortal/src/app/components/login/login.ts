import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { LoginModel } from '../../models/login.model';
import { AuthApiService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  loginModel: LoginModel = new LoginModel();

  progress = false;

  constructor(
    private authApiService: AuthApiService,
    private router: Router
  ) {}

  loginClick() {

    if (
      this.loginModel.username.trim() === '' ||
      this.loginModel.password.trim() === ''
    ) {
      alert('Username and Password are required');
      return;
    }

    if (this.loginModel.username.length < 4) {
      alert('Username must be at least 4 characters long');
      return;
    }

    this.progress = true;
    this.authApiService.loginApiCall(this.loginModel).subscribe({
      next: (response) => {

        sessionStorage.setItem('token', response.accessToken);

        this.authApiService.setCurrentUser(response);
        alert('Login successful');

        this.router.navigate(['/dashboard/header']);
      },

      error: (error) => {

        console.error('Login failed', error);

        alert('Invalid username or password');

        this.progress = false;
      }
    });
  }
}