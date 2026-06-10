import { Component,inject } from '@angular/core';
import { AuthApiService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
   authService = inject(AuthApiService);
}

