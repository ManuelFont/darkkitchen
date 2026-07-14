import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { filter, finalize, map, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { CategoryDropdownComponent } from '../category-dropdown/category-dropdown.component';
import { Product } from '../models/product.model';
import { UpdateProductRequest } from '../models/update-product-request.model';
import { ProductsService } from '../services/products.service';

@Component({
  selector: 'app-product-details',
  imports: [
    CategoryDropdownComponent,
    FormsModule,
    MatIconModule,
    RouterLink,
    SidebarComponent,
  ],
  templateUrl: './product-details.component.html',
})
export class ProductDetailsComponent {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly productsService = inject(ProductsService);
  private readonly router = inject(Router);

  readonly product = signal<Product | undefined>(undefined);
  readonly isSubmitting = signal(false);
  readonly isDeleting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly hasAttemptedSubmit = signal(false);

  imageUrl1 = '';
  imageUrl2 = '';
  imageUrl3 = '';

  constructor() {
    this.activatedRoute.paramMap
      .pipe(
        map((params) => params.get('id')),
        filter((productId): productId is string => productId !== null),
        switchMap((productId) => this.productsService.getById(productId)),
        takeUntilDestroyed(),
      )
      .subscribe((product) => {
        this.product.set(product);
        this.imageUrl1 = product.imagesUrls[0] ?? '';
        this.imageUrl2 = product.imagesUrls[1] ?? '';
        this.imageUrl3 = product.imagesUrls[2] ?? '';
      });
  }

  onSubmit(isFormValid: boolean | null): void {
    if (this.isSubmitting() || this.isDeleting()) {
      return;
    }

    this.hasAttemptedSubmit.set(true);
    this.errorMessage.set(null);
    const product = this.product();

    if (!product || !isFormValid || !product.category.id) {
      this.errorMessage.set('Complete all required fields');
      return;
    }

    this.updateProduct(product.id, this.createRequest(product));
  }

  deleteProduct(productId: string): void {
    if (this.isDeleting() || this.isSubmitting()) {
      return;
    }

    this.errorMessage.set(null);
    this.isDeleting.set(true);

    this.productsService
      .delete(productId)
      .pipe(finalize(() => this.isDeleting.set(false)))
      .subscribe({
        next: () => {
          void this.router.navigate(['/products'], {
            state: { successMessage: 'Product deleted successfully' },
          });
        },
        error: (error: Error) => this.errorMessage.set(error.message),
      });
  }

  selectCategory(categoryId: string): void {
    const product = this.product();
    if (!product) {
      return;
    }

    product.category.id = categoryId;
  }

  private updateProduct(productId: string, request: UpdateProductRequest): void {
    this.isSubmitting.set(true);

    this.productsService
      .update(productId, request)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          void this.router.navigate(['/products'], {
            state: { successMessage: 'Product updated successfully' },
          });
        },
        error: (error: Error) => this.errorMessage.set(error.message),
      });
  }

  private createRequest(product: Product): UpdateProductRequest {
    return {
      name: product.name.trim(),
      description: product.description.trim(),
      imagesUrls: [this.imageUrl1, this.imageUrl2, this.imageUrl3]
        .map((imageUrl: string) => imageUrl.trim())
        .filter((imageUrl: string) => imageUrl.length > 0),
      price: product.price,
      categoryId: product.category.id,
    };
  }
}
