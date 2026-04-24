import { Routes } from '@angular/router';
import { HomePageComponent } from './pages/home/home-page.component';
import { AuthPageComponent } from './pages/auth/auth-page.component';
import { BookingPageComponent } from './pages/booking/booking-page.component';
import { HistoryPageComponent } from './pages/history/history-page.component';
import { ProfilePageComponent } from './pages/profile/profile-page.component';
import { OperatorPageComponent } from './pages/operator/operator-page.component';
import { AdminPageComponent } from './pages/admin/admin-page.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'auth', component: AuthPageComponent },
  { path: 'booking/:tripId', component: BookingPageComponent, canActivate: [authGuard] },
  {
    path: 'history',
    component: HistoryPageComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['User'] }
  },
  {
    path: 'profile',
    component: ProfilePageComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['User'] }
  },
  {
    path: 'operator',
    component: OperatorPageComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Operator'] }
  },
  {
    path: 'admin',
    component: AdminPageComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] }
  },
  { path: '**', redirectTo: '' }
];
