import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, EMPTY, Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';

const unauthorizedStatusCode = 401;
const notFoundStatusCode = 404;
const networkErrorStatusCode = 0;
const internalServerErrorStatusCode = 500;
const sessionsUrl = `${environment.apiBaseUrl}/sessions`;
const registrationUrl = `${environment.apiBaseUrl}/users/register`;

export function failureResponseInterceptor(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> {
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === unauthorizedStatusCode) {
        authService.logout();
        void router.navigateByUrl('/login');
        return EMPTY;
      }

      if (error.status === notFoundStatusCode) {
        void router.navigateByUrl('/not-found', {
          skipLocationChange: true,
        });
        return EMPTY;
      }

      const shouldNavigateToServerError =
        (error.status === networkErrorStatusCode ||
          error.status === internalServerErrorStatusCode) &&
        !isLoginOrRegisterRequest(request);

      if (shouldNavigateToServerError) {
        void router.navigateByUrl('/server-error', {
          skipLocationChange: true,
        });
        return EMPTY;
      }

      return throwError(() => error);
    }),
  );
}

function isLoginOrRegisterRequest(request: HttpRequest<unknown>): boolean {
  return (
    request.method === 'POST' && (request.url === sessionsUrl || request.url === registrationUrl)
  );
}
