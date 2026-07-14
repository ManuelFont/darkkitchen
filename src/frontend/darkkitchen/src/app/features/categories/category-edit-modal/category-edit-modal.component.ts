import { Component, inject, OnInit, output, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { CategoryCreateRowComponent } from '../category-create-row/category-create-row.component';
import { CategoryEditRowComponent } from '../category-edit-row/category-edit-row.component';
import { Category } from '../models/category.model';
import { CategoriesService } from '../services/categories.service';

@Component({
  selector: 'app-category-edit-modal',
  imports: [CategoryCreateRowComponent, CategoryEditRowComponent],
  templateUrl: './category-edit-modal.component.html',
})
export class CategoryEditModalComponent implements OnInit {
  private readonly categoriesService = inject(CategoriesService);

  readonly closeRequested = output<void>();
  readonly categories = signal<Category[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  reloadCategories(): void {
    this.loadCategories();
  }

  private loadCategories(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.categoriesService
      .getAll()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (categories) => this.categories.set(categories),
        error: () => this.errorMessage.set('Failed to load categories'),
      });
  }
}
