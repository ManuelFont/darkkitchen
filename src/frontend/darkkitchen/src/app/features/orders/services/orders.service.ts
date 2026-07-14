import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { CreateOrderRequest } from '../models/create-order-request.model';
import { CreateOrderResponse } from '../models/create-order-response.model';
import { OrderAction, ORDER_ACTIONS } from '../models/order-action';
import { OrderDetail } from '../models/order-detail.model';
import { OrderFilters } from '../models/order-filters.model';
import { OrderListItem } from '../models/order-list-item.model';
import { OrderStatusChangeResponse } from '../models/order-status-change-response.model';
import { ClientOrderSummary } from '../models/order-summary.model';

const defaultGetMyOrdersErrorMessage = 'Could not load your orders';
const defaultGetOrdersErrorMessage = 'Could not load orders';
const defaultGetOrderErrorMessage = 'Could not load the order';
const defaultCreateOrderErrorMessage = 'Could not place your order';
const defaultChangeStatusErrorMessage = 'Could not update the order';

const ACTION_ENDPOINTS: Record<OrderAction, string> = ORDER_ACTIONS.reduce(
  (map, descriptor) => ({ ...map, [descriptor.action]: descriptor.endpoint }),
  {} as Record<OrderAction, string>,
);

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  private readonly http = inject(HttpClient);
  private readonly ordersUrl = `${environment.apiBaseUrl}/orders`;

  public getMyOrders(filters: OrderFilters): Observable<ClientOrderSummary[]> {
    return this.http
      .get<ClientOrderSummary[]>(`${this.ordersUrl}/my`, { params: this.buildParams(filters) })
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetMyOrdersErrorMessage),
        ),
      );
  }

  public getAllOrders(filters: OrderFilters): Observable<OrderListItem[]> {
    return this.http
      .get<OrderListItem[]>(this.ordersUrl, { params: this.buildParams(filters) })
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetOrdersErrorMessage),
        ),
      );
  }

  public changeStatus(orderId: string, action: OrderAction): Observable<OrderStatusChangeResponse> {
    const url = `${this.ordersUrl}/${orderId}${ACTION_ENDPOINTS[action]}`;
    return this.http
      .patch<OrderStatusChangeResponse>(url, {})
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultChangeStatusErrorMessage),
        ),
      );
  }

  public getById(orderId: string): Observable<OrderDetail> {
    return this.http
      .get<OrderDetail>(`${this.ordersUrl}/${orderId}`)
      .pipe(
        catchError((error: HttpErrorResponse) => this.mapError(error, defaultGetOrderErrorMessage)),
      );
  }

  public create(request: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this.http
      .post<CreateOrderResponse>(this.ordersUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultCreateOrderErrorMessage),
        ),
      );
  }

  private buildParams({ dateFrom, dateTo, status }: OrderFilters): HttpParams {
    let params = new HttpParams();

    if (dateFrom) {
      params = params.set('dateFrom', dateFrom);
    }

    if (dateTo) {
      params = params.set('dateTo', dateTo);
    }

    if (status) {
      params = params.set('status', status);
    }

    return params;
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
