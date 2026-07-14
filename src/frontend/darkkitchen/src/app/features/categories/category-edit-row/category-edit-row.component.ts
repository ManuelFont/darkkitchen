import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { Category } from '../models/category.model';
import { UpdateCategoryRequest } from '../models/update-category-request.model';
import { CategoriesService } from '../services/categories.service';

type CategoryStatus = 'idle' | 'saving' | 'deleting';

@Component({
  selector: 'app-category-edit-row',
  imports: [FormsModule],
  templateUrl: './category-edit-row.component.html',
})
export class CategoryEditRowComponent implements OnInit {
  private readonly categoriesService = inject(CategoriesService);

  readonly category = input.required<Category>();
  readonly categoryChanged = output<void>();
  readonly hasChanges = signal(false);
  readonly status = signal<CategoryStatus>('idle');
  readonly errorMessage = signal<string | null>(null);

  name = '';
  description = '';

  ngOnInit(): void {
    this.name = this.category().name;
    this.description = this.category().description;
  }

  markAsChanged(): void {
    this.hasChanges.set(true);
  }

  updateCategory(): void {
    if (this.status() !== 'idle') {
      return;
    }

    const request = this.createUpdateRequest();
    if (!request.name || !request.description) {
      this.errorMessage.set('Name and description are required');
      return;
    }

    this.startOperation('saving');
    this.categoriesService
      .update(this.category().id, request)
      .pipe(finalize(() => this.finishOperation()))
      .subscribe({
        next: () => this.categoryChanged.emit(),
        error: (error: Error) => this.showError(error),
      });
  }

  deleteCategory(): void {
    if (this.status() !== 'idle') {
      return;
    }

    this.startOperation('deleting');
    this.categoriesService
      .delete(this.category().id)
      .pipe(finalize(() => this.finishOperation()))
      .subscribe({
        next: () => this.categoryChanged.emit(),
        error: (error: Error) => this.showError(error),
      });
  }

  private createUpdateRequest(): UpdateCategoryRequest {
    return {
      name: this.name.trim(),
      description: this.description.trim(),
    };
  }

  private startOperation(status: CategoryStatus): void {
    this.status.set(status);
    this.errorMessage.set(null);
  }

  private finishOperation(): void {
    this.status.set('idle');
  }

  private showError(error: Error): void {
    this.errorMessage.set(error.message);
  }
}
