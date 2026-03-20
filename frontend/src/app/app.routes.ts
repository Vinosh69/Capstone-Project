import { Routes } from '@angular/router';
import { Home } from './components/home/home';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { RenterDashboard } from './components/renter-dashboard/renter-dashboard';
import { OwnerDashboard } from './components/owner-dashboard/owner-dashboard';
import { PropertyDetails } from './components/property-details/property-details';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'renter', component: RenterDashboard },
  { path: 'owner', component: OwnerDashboard },
  { path: 'property/:id', component: PropertyDetails },
  { path: '**', redirectTo: '' }
];