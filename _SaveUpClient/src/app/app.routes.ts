import { Routes } from '@angular/router';
import { logoutRouteGuard } from './_auth/guard/logout-route.guard';
import { NotFoundComponent } from './common/not-found/not-found.component';
import { authGuard } from './_auth/guard/auth.guard';

export const routes: Routes = [
    {
        path: 'home',
        pathMatch: 'full',
        canActivate: [authGuard],
        loadComponent : () =>
            import('./main/main.component')
        .then((mod) => mod.MainComponent)
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
