import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { getApiErrorMessage } from '../../../core/http/api-error';
import { CreateProductRequest } from '../models/create-product-request.model';
import { ProductFilters } from '../models/product-filters.model';
import { Product } from '../models/product.model';
import { UpdateProductRequest } from '../models/update-product-request.model';

const defaultGetProductsErrorMessage = 'Could not load products';
const defaultGetProductErrorMessage = 'Could not load product';
const defaultCreateProductErrorMessage = 'Could not create product';
const defaultUpdateProductErrorMessage = 'Could not update product';
const defaultApplyPromotionErrorMessage = 'Could not apply promotion to product';
const defaultRemovePromotionErrorMessage = 'Could not remove promotion from product';
const defaultDeleteProductErrorMessage = 'Could not delete product';

@Injectable({
  providedIn: 'root',
})
export class ProductsService {
  private readonly http = inject(HttpClient);
  private readonly productsUrl = `${environment.apiBaseUrl}/products`;

  public getAll(filters: ProductFilters = {}): Observable<Product[]> {
    return this.http
      .get<Product[]>(this.productsUrl, {
        params: this.createParams(filters),
      })
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetProductsErrorMessage),
        ),
      );
  }

  public getById(productId: string): Observable<Product> {
    return this.http
      .get<Product>(`${this.productsUrl}/${productId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultGetProductErrorMessage),
        ),
      );
  }

  public create(request: CreateProductRequest): Observable<Product> {
    return this.http
      .post<Product>(this.productsUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultCreateProductErrorMessage),
        ),
      );
  }

  public update(productId: string, request: UpdateProductRequest): Observable<void> {
    return this.http
      .put<void>(`${this.productsUrl}/${productId}`, request)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultUpdateProductErrorMessage),
        ),
      );
  }

  public delete(productId: string): Observable<void> {
    return this.http
      .delete<void>(`${this.productsUrl}/${productId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultDeleteProductErrorMessage),
        ),
      );
  }

  public applyPromotion(productId: string, promotionId: string): Observable<void> {
    return this.http
      .post<void>(`${this.productsUrl}/${productId}/promotions/${promotionId}`, {})
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultApplyPromotionErrorMessage),
        ),
      );
  }

  public removePromotion(productId: string, promotionId: string): Observable<void> {
    return this.http
      .delete<void>(`${this.productsUrl}/${productId}/promotions/${promotionId}`)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          this.mapError(error, defaultRemovePromotionErrorMessage),
        ),
      );
  }

  private createParams({ categoryId, name }: ProductFilters): HttpParams {
    let params = new HttpParams();

    if (categoryId) {
      params = params.set('categoryId', categoryId);
    }

    if (name) {
      params = params.set('name', name);
    }

    return params;
  }

  private mapError(error: HttpErrorResponse, fallbackMessage: string): Observable<never> {
    return throwError(() => new Error(getApiErrorMessage(error, fallbackMessage)));
  }
}
