import { Component } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
// import { Header } from '../header/header';
import { logout } from '../../rxjs/auth.operator';
import { AuthApiService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [ RouterLink, RouterOutlet],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {

  constructor(private router: Router, public authService: AuthApiService) {}

  logout() {

    logout();
    this.authService.currentUser.set(null);

    this.router.navigate(['/']);

  }
}