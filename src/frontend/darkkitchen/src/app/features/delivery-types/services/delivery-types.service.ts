import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { DeliveryType } from '../models/delivery-type.model';
import { DeliveryTypeRequest } from '../models/delivery-type-request.model';

const defaultGetDeliveryTypesErrorMessage = 'Could not load delivery types';
const defaultGetDeliveryTypeErrorMessage = 'Could not load delivery type';
const defaultCreateDeliveryTypeErrorMessage = 'Could not create delivery type';
const defaultUpdateDeliveryTypeErrorMessage = 'Could not update delivery type';

@Injectable({
  providedIn: 'root',
})
export class DeliveryTypesService {
  private readonly http = inject(HttpClient);
  private readonly deliveryTypesUrl = `${environment.apiBaseUrl}/delivery-types`;

  public getAll(): Observable<DeliveryType[]> {
    return this.http
      .get<DeliveryType[]>(this.deliveryTypesUrl)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetDeliveryTypesErrorMessage),
        ),
      );
  }

  public getById(deliveryTypeId: string): Observable<DeliveryType> {
    return this.http
      .get<DeliveryType>(`${this.deliveryTypesUrl}/${deliveryTypeId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetDeliveryTypeErrorMessage),
        ),
      );
  }

  public create(request: DeliveryTypeRequest): Observable<DeliveryType> {
    return this.http
      .post<DeliveryType>(this.deliveryTypesUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultCreateDeliveryTypeErrorMessage),
        ),
      );
  }

  public update(deliveryTypeId: string, request: DeliveryTypeRequest): Observable<void> {
    return this.http
      .put<void>(`${this.deliveryTypesUrl}/${deliveryTypeId}`, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultUpdateDeliveryTypeErrorMessage),
        ),
      );
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
