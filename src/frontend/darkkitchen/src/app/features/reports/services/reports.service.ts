import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { DateRange } from '../models/date-range.model';
import { SalesReport } from '../models/sales-report.model';
import { TopProduct } from '../models/top-product.model';

const defaultTopProductsErrorMessage = 'Could not load top products';
const defaultSalesReportErrorMessage = 'Could not load sales report';

@Injectable({
  providedIn: 'root',
})
export class ReportsService {
  private readonly http = inject(HttpClient);
  private readonly reportsUrl = `${environment.apiBaseUrl}/reports`;

  public getTopProducts(range: DateRange): Observable<TopProduct[]> {
    return this.http
      .get<TopProduct[]>(`${this.reportsUrl}/top-products`, { params: this.buildParams(range) })
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultTopProductsErrorMessage),
        ),
      );
  }

  public getSalesReport(range: DateRange): Observable<SalesReport> {
    return this.http
      .get<SalesReport>(`${this.reportsUrl}/sales`, { params: this.buildParams(range) })
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultSalesReportErrorMessage),
        ),
      );
  }

  private buildParams({ dateFrom, dateTo }: DateRange): HttpParams {
    let params = new HttpParams();

    if (dateFrom) {
      params = params.set('dateFrom', dateFrom);
    }

    if (dateTo) {
      params = params.set('dateTo', dateTo);
    }

    return params;
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
