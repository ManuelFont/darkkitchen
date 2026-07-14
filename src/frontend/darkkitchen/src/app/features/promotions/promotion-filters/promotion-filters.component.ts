import { Component, output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { PromotionFilters } from '../models/promotion-filters.model';

@Component({
  selector: 'app-promotion-filters',
  imports: [ReactiveFormsModule],
  templateUrl: './promotion-filters.component.html',
})
export class PromotionFiltersComponent {
  readonly filtersChange = output<PromotionFilters>();

  protected readonly form = new FormGroup({
    search: new FormControl('', { nonNullable: true }),
  });

  constructor() {
    this.form.valueChanges
      .pipe(debounceTime(250), takeUntilDestroyed())
      .subscribe(() => this.filtersChange.emit(this.form.getRawValue()));
  }
}
