import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Dashboard } from './components/dashboard/dashboard';
// import { Dashboar } from './components/home/home';
import { Products } from './components/products/products';
import { ProductDetails } from './components/product-details/product-details';
import { Profile } from './components/profile/profile';
import { authGuard } from './guards/auth.guard';
import { Header } from './components/header/header';

export const routes: Routes = [

  {
    path: '',
    component: Dashboard
    // component: Login
  },

  {
    path: 'login',
    component: Login
  }
,
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard],
    children: [

      {
        path: 'header',
        component: Header
      },

      {
        path: 'products',
        component: Products
      },

      {
        path: 'product-details/:id',
        component: ProductDetails
      },

      {
        path: 'profile',
        component: Profile
      },

      {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full'
      }
    ]
  },

  {
    path: '**',
    redirectTo: ''
  }
];