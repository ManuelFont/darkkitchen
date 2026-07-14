import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { ProductImportResult } from '../models/product-import-result.model';
import { ProductImporter } from '../models/product-importer.model';

const defaultGetImportersErrorMessage = 'Could not load product importers';
const defaultImportProductsErrorMessage = 'Could not import products';

@Injectable({
  providedIn: 'root',
})
export class ProductImportsService {
  private readonly http = inject(HttpClient);
  private readonly productImportsUrl = `${environment.apiBaseUrl}/product-imports`;

  public getImporters(): Observable<ProductImporter[]> {
    return this.http
      .get<ProductImporter[]>(`${this.productImportsUrl}/importers`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetImportersErrorMessage),
        ),
      );
  }

  public importProducts(importerId: string, file: File): Observable<ProductImportResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http
      .post<ProductImportResult>(
        `${this.productImportsUrl}/importers/${importerId}/import`,
        formData,
      )
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultImportProductsErrorMessage),
        ),
      );
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
