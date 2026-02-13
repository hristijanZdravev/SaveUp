import { Routes } from '@angular/router';
import { logoutRouteGuard } from './_auth/guard/logout-route.guard';
import { NotFoundComponent } from './common/not-found/not-found.component';
import { authGuard } from './_auth/guard/auth.guard';

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
    },
    {
        path: 'dashboard',
        canActivate: [authGuard],
        loadComponent : () =>
            import('./user/dashboard/dashboard.component')
        .then((mod) => mod.DashboardComponent)
    },
    {
        path: 'workouts',
        canActivate: [authGuard],
        loadComponent : () =>
            import('./user/workouts/workouts.component')
        .then((mod) => mod.WorkoutsComponent)
    },
    {
        path: 'workouts/:id',
        canActivate: [authGuard],
        loadComponent : () =>
            import('./user/workouts/workout-detail/workout-detail.component')
        .then((mod) => mod.WorkoutDetailComponent)
    },
    {
        path: 'progress',
        canActivate: [authGuard],
        loadComponent : () =>
            import('./user/progress/progress.component')
        .then((mod) => mod.ProgressComponent)
    },
    {
        path: 'logout',
        canActivate: [logoutRouteGuard],
        loadComponent : () =>
            import('./common/logout/logout.component')
        .then((mod) => mod.LogoutComponent)      
    },
      {
        path: '404',
        component: NotFoundComponent,
      },
      {
        path: '**',
        component: NotFoundComponent,
      },
];
