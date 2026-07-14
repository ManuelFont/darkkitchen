import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';

export function authorizationHeaderInterceptor(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> {
  if (!request.url.startsWith(environment.apiBaseUrl)) {
    return next(request);
  }

  const authService = inject(AuthService);
  return next(authService.addAuthorizationHeader(request));
}
