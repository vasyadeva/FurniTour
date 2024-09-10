import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthInterceptor } from './services/app.interceptor';
import { AuthGuard } from './services/app.auth.guard';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes),    {
    provide: HTTP_INTERCEPTORS,
    useFactory: function (router: Router) {
        return new AuthInterceptor(router);
    },
    multi: true,
    deps: [Router]
  },
  AuthGuard,
  provideHttpClient()]
};
