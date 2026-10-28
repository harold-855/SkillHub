import { Routes } from '@angular/router';
import { WorkshopListComponent } from './features/workshops/workshop-list/workshop-list';
import { WorkshopDetailComponent } from './features/workshops/workshop-detail/workshop-detail';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { MyEnrollmentsComponent } from './features/enrollments/my-enrollments/my-enrollments';
import { authGuard } from './core/guards/auth.guard';

/**
 * Main application routing configuration.
 * Maps URLs to standalone feature components and applies guards where authentication is required.
 */
export const routes: Routes = [
  // Default redirect to public workshop catalog
  { path: '', redirectTo: 'workshops', pathMatch: 'full' },

  // Public catalog and detailed views
  { path: 'workshops', component: WorkshopListComponent },
  { path: 'workshops/:id', component: WorkshopDetailComponent },

  // Authentication routes
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Protected participant view
  { path: 'my-enrollments', component: MyEnrollmentsComponent, canActivate: [authGuard] },

  // Wildcard fallback
  { path: '**', redirectTo: 'workshops' }
];
