import { APP_INITIALIZER, ApplicationConfig, Provider, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { KeycloakAngularModule, KeycloakBearerInterceptor, KeycloakService } from 'keycloak-angular';

function initializeKeycloak(keycloak: KeycloakService)  {
    return () =>
      keycloak.init({
        config: {
          url: 'http://localhost:8080',
          realm: 'gym-realm',
          clientId: 'GymClient',
        },
        initOptions: {
          onLoad: 'check-sso',
          checkLoginIframe: false,
          silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html',
        },
        loadUserProfileAtStartUp: true,
        enableBearerInterceptor: true,
        bearerPrefix: 'Bearer ',
    });
}

// Provider for Keycloak Bearer Interceptor
const KeycloakBearerInterceptorProvider: Provider = {
    provide: HTTP_INTERCEPTORS,
    useClass: KeycloakBearerInterceptor,
    multi: true,
  };

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    KeycloakAngularModule,
    KeycloakBearerInterceptorProvider,
    KeycloakService,
    {
        provide: APP_INITIALIZER,
        useFactory: initializeKeycloak,
        multi: true,
        deps: [KeycloakService]
    },
  ]
};
