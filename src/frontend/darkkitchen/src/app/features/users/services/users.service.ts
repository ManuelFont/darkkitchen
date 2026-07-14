import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { CreateUserRequest } from '../models/create-user-request.model';
import { UpdateUserRequest } from '../models/update-user-request.model';
import { UserFilters } from '../models/user-filters.model';
import { User } from '../models/user.model';

const defaultGetUsersErrorMessage = 'Could not load users';
const defaultCreateUserErrorMessage = 'Could not create user';
const defaultUpdateUserErrorMessage = 'Could not update user';
const defaultDeleteUserErrorMessage = 'Could not delete user';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly http = inject(HttpClient);
  private readonly usersUrl = `${environment.apiBaseUrl}/users`;

  public getAll(filters: UserFilters = { search: '', role: '' }): Observable<User[]> {
    return this.http
      .get<User[]>(this.usersUrl, { params: this.buildParams(filters) })
      .pipe(
        catchError((error: HttpErrorResponse) => this.mapError(error, defaultGetUsersErrorMessage)),
      );
  }

  private buildParams({ search, role }: UserFilters): HttpParams {
    let params = new HttpParams();

    if (search) {
      params = params.set('search', search);
    }

    if (role) {
      params = params.set('role', role);
    }

    return params;
  }

  public create(request: CreateUserRequest): Observable<User> {
    return this.http
      .post<User>(this.usersUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultCreateUserErrorMessage),
        ),
      );
  }

  public update(userId: string, request: UpdateUserRequest): Observable<User> {
    return this.http
      .put<User>(`${this.usersUrl}/${userId}`, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultUpdateUserErrorMessage),
        ),
      );
  }

  public delete(userId: string): Observable<void> {
    return this.http
      .delete<void>(`${this.usersUrl}/${userId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultDeleteUserErrorMessage),
        ),
      );
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
