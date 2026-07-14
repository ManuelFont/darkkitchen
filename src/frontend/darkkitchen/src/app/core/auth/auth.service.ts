import { HttpClient, HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginResponse } from '../../features/auth/models/login-response.model';
import { LoginRequest } from '../../features/auth/models/login-request.model';
import { RegisterRequest } from '../../features/auth/models/register-request.model';
import { RegisterResponse } from '../../features/auth/models/register-response.model';
import { sessionRoleKey, sessionTokenKey } from '../constants/storage-keys';
import { getApiErrorMessage } from '../http/api-error';
import { Role } from './role.model';

const defaultLoginErrorMessage = 'Log in failed';
const defaultRegistrationErrorMessage = 'Registration failed';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly sessionsUrl = `${environment.apiBaseUrl}/sessions`;
  private readonly registrationUrl = `${environment.apiBaseUrl}/users/register`;
  private readonly role = signal<Role | null>(
    localStorage.getItem(sessionRoleKey) as Role | null,
  );

  public readonly currentRole = this.role.asReadonly();

  public createSession(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.sessionsUrl, request).pipe(
      tap((response: LoginResponse) => {
        this.storeToken(response.token);
        this.storeRole(response.role);
      }),
      catchError((error: HttpErrorResponse) => this.mapError(error, defaultLoginErrorMessage)),
    );
  }

  public register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http
      .post<RegisterResponse>(this.registrationUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultRegistrationErrorMessage),
        ),
      );
  }

  public logout(): void {
    localStorage.removeItem(sessionTokenKey);
    localStorage.removeItem(sessionRoleKey);
    this.role.set(null);
  }

  public hasSession(): boolean {
    return localStorage.getItem(sessionTokenKey) !== null;
  }

  public hasRole(...roles: Role[]): boolean {
    const current = this.role();
    return current !== null && roles.includes(current);
  }

  public addAuthorizationHeader<T>(request: HttpRequest<T>): HttpRequest<T> {
    const token = this.getToken();

    if (!token) {
      return request;
    }

    return request.clone({
      setHeaders: {
        Authorization: token,
      },
    });
  }

  private storeToken(token: string): void {
    localStorage.setItem(sessionTokenKey, token);
  }

  private storeRole(role: Role): void {
    localStorage.setItem(sessionRoleKey, role);
    this.role.set(role);
  }

  private getToken(): string {
    return localStorage.getItem(sessionTokenKey) ?? '';
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
