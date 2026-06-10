import { Component, inject } from '@angular/core';
import { AuthApiService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})

export class Profile {

  authService = inject(AuthApiService);

}