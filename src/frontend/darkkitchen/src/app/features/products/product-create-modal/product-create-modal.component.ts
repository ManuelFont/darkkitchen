import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { CategoryDropdownComponent } from '../category-dropdown/category-dropdown.component';
import { CreateProductRequest } from '../models/create-product-request.model';
import { Product } from '../models/product.model';
import { ProductsService } from '../services/products.service';

@Component({
  selector: 'app-product-create-modal',
  imports: [CategoryDropdownComponent, FormsModule],
  templateUrl: './product-create-modal.component.html',
})
export class ProductCreateModalComponent {
  private readonly productsService = inject(ProductsService);

  readonly closeRequested = output<void>();
  readonly productCreated = output<Product>();
  readonly selectedCategoryId = signal('');
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly hasAttemptedSubmit = signal(false);

  name = '';
  description = '';
  imageUrl1 = '';
  imageUrl2 = '';
  imageUrl3 = '';
  price: number | null = null;

  onSubmit(isFormValid: boolean | null): void {
    if (this.isSubmitting()) {
      return;
    }

    this.hasAttemptedSubmit.set(true);
    this.errorMessage.set(null);
    const price = this.price;

    if (!isFormValid || !this.selectedCategoryId() || price === null) {
      this.errorMessage.set('Complete all required fields');
      return;
    }

    this.submitProduct(this.createRequest(price));
  }

  private submitProduct(request: CreateProductRequest): void {
    this.isSubmitting.set(true);

    this.productsService
      .create(request)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (product: Product) => this.productCreated.emit(product),
        error: (error: Error) => this.errorMessage.set(error.message),
      });
  }

  private createRequest(price: number): CreateProductRequest {
    return {
      name: this.name.trim(),
      description: this.description.trim(),
      imagesUrls: [this.imageUrl1, this.imageUrl2, this.imageUrl3]
        .map((imageUrl: string) => imageUrl.trim())
        .filter((imageUrl: string) => imageUrl.length > 0),
      price,
      categoryId: this.selectedCategoryId(),
    };
  }
}
