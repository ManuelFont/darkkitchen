import { Component, inject, input, output } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs';
import { CategoriesService } from '../../categories/services/categories.service';

@Component({
  selector: 'app-category-dropdown',
  imports: [],
  templateUrl: './category-dropdown.component.html',
})
export class CategoryDropdownComponent {
  private readonly categoriesService = inject(CategoriesService);

  readonly refreshVersion = input(0);
  readonly categories = toSignal(
    toObservable(this.refreshVersion).pipe(switchMap(() => this.categoriesService.getAll())),
    { initialValue: [] },
  );
  readonly disabledOptionName = input.required<string>();
  readonly isFirstOptionDisabled = input.required<boolean>();
  readonly selectedCategoryId = input('');
  readonly categorySelected = output<string>();

  selectCategory(event: Event): void {
    const categoryId = (event.target as HTMLSelectElement).value;
    this.categorySelected.emit(categoryId);
  }
}
