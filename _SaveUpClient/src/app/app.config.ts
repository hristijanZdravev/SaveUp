import { APP_INITIALIZER, ApplicationConfig, Provider, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { KeycloakAngularModule, KeycloakBearerInterceptor, KeycloakService } from 'keycloak-angular';
import { environment } from '../environments/environment';
import { LocaleService } from './_services/locale.service';

function initializeKeycloak(keycloak: KeycloakService, localeService: LocaleService)  {
    return async () => {
      localeService.applyLocale('en');
      await keycloak.init({
        config: {
          url: environment.keycloakUrl,
          realm: environment.keycloakRealm,
          clientId: environment.keycloakClientId,
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
      const tokenLocale = (keycloak.getKeycloakInstance().tokenParsed as { locale?: string } | undefined)?.locale;
      localeService.applyLocale(tokenLocale);
    };
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
        deps: [KeycloakService, LocaleService]
    },
  ]
};
