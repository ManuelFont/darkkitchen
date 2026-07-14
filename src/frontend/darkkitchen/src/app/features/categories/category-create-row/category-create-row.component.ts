import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { CreateCategoryRequest } from '../models/create-category-request.model';
import { CategoriesService } from '../services/categories.service';

const categoryCreatedSuccessfullyMessage = 'Category created successfully';

@Component({
  selector: 'app-category-create-row',
  imports: [FormsModule],
  templateUrl: './category-create-row.component.html',
})
export class CategoryCreateRowComponent {
  private readonly categoriesService = inject(CategoriesService);

  readonly categoryCreated = output<void>();
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);
  readonly isCreating = signal(false);

  name = '';
  description = '';

  clearMessages(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  createCategory(): void {
    if (this.isCreating()) {
      return;
    }

    const request = this.createRequest();
    if (!request.name || !request.description) {
      this.successMessage.set(null);
      this.errorMessage.set('Name and description are required');
      return;
    }

    this.clearMessages();
    this.isCreating.set(true);

    this.categoriesService
      .create(request)
      .pipe(finalize(() => this.isCreating.set(false)))
      .subscribe({
        next: () => {
          this.clearFields();
          this.successMessage.set(categoryCreatedSuccessfullyMessage);
          this.categoryCreated.emit();
        },
        error: (error: Error) => {
          this.successMessage.set(null);
          this.errorMessage.set(error.message);
        },
      });
  }

  private createRequest(): CreateCategoryRequest {
    return {
      name: this.name.trim(),
      description: this.description.trim(),
    };
  }

  private clearFields(): void {
    this.name = '';
    this.description = '';
  }
}
