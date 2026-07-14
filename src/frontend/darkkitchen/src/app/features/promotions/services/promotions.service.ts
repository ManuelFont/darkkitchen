import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { Promotion } from '../models/promotion.model';
import { PromotionRequest } from '../models/promotion-request.model';

const defaultGetPromotionsErrorMessage = 'Could not load promotions';
const defaultGetPromotionErrorMessage = 'Could not load promotion';
const defaultCreatePromotionErrorMessage = 'Could not create promotion';
const defaultUpdatePromotionErrorMessage = 'Could not update promotion';
const defaultDeletePromotionErrorMessage = 'Could not delete promotion';

@Injectable({
  providedIn: 'root',
})
export class PromotionsService {
  private readonly http = inject(HttpClient);
  private readonly promotionsUrl = `${environment.apiBaseUrl}/promotions`;

  public getAll(): Observable<Promotion[]> {
    return this.http
      .get<Promotion[]>(this.promotionsUrl)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetPromotionsErrorMessage),
        ),
      );
  }

  public getById(promotionId: string): Observable<Promotion> {
    return this.http
      .get<Promotion>(`${this.promotionsUrl}/${promotionId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetPromotionErrorMessage),
        ),
      );
  }

  public create(request: PromotionRequest): Observable<Promotion> {
    return this.http
      .post<Promotion>(this.promotionsUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultCreatePromotionErrorMessage),
        ),
      );
  }

  public update(promotionId: string, request: PromotionRequest): Observable<Promotion> {
    return this.http
      .put<Promotion>(`${this.promotionsUrl}/${promotionId}`, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultUpdatePromotionErrorMessage),
        ),
      );
  }

  public delete(promotionId: string): Observable<void> {
    return this.http
      .delete<void>(`${this.promotionsUrl}/${promotionId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultDeletePromotionErrorMessage),
        ),
      );
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
