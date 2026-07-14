import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { Category } from '../models/category.model';
import { CreateCategoryRequest } from '../models/create-category-request.model';
import { UpdateCategoryRequest } from '../models/update-category-request.model';

const defaultGetErrorMessage = 'Could not load categories';
const defaultCreateErrorMessage = 'Could not create category';
const defaultUpdateErrorMessage = 'Could not update category';
const defaultDeleteErrorMessage = 'Could not delete category';

@Injectable({
  providedIn: 'root',
})
export class CategoriesService {
  private readonly http = inject(HttpClient);
  private readonly categoriesUrl = `${environment.apiBaseUrl}/categories`;

  public getAll(): Observable<Category[]> {
    return this.http
      .get<Category[]>(this.categoriesUrl)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          throwError(() => new Error(getApiErrorMessage(error, defaultGetErrorMessage))),
        ),
      );
  }

  public create(request: CreateCategoryRequest): Observable<Category> {
    return this.http
      .post<Category>(this.categoriesUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          throwError(() => new Error(getApiErrorMessage(error, defaultCreateErrorMessage))),
        ),
      );
  }

  public delete(categoryId: string): Observable<void> {
    return this.http
      .delete<void>(`${this.categoriesUrl}/${categoryId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          throwError(() => new Error(getApiErrorMessage(error, defaultDeleteErrorMessage))),
        ),
      );
  }

  public update(categoryId: string, request: UpdateCategoryRequest): Observable<Category> {
    return this.http
      .put<Category>(`${this.categoriesUrl}/${categoryId}`, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          throwError(() => new Error(getApiErrorMessage(error, defaultUpdateErrorMessage))),
        ),
      );
  }
}
