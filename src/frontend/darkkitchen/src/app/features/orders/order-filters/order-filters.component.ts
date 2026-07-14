import { Component, computed, output } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { debounceTime, startWith } from 'rxjs';
import { OrderFilters } from '../models/order-filters.model';
import { ORDER_STATUS_OPTIONS } from '../models/order-status';

@Component({
  selector: 'app-order-filters',
  imports: [MatIconModule, ReactiveFormsModule],
  templateUrl: './order-filters.component.html',
})
export class OrderFiltersComponent {
  protected readonly statuses = ORDER_STATUS_OPTIONS;

  readonly filtersChange = output<OrderFilters>();

  protected readonly form = new FormGroup({
    dateFrom: new FormControl('', { nonNullable: true }),
    dateTo: new FormControl('', { nonNullable: true }),
    status: new FormControl('', { nonNullable: true }),
  });

  private readonly value = toSignal(
    this.form.valueChanges.pipe(startWith(this.form.getRawValue())),
    { initialValue: this.form.getRawValue() },
  );

  protected readonly hasValue = computed(() => {
    const { dateFrom, dateTo, status } = this.value();
    return Boolean(dateFrom) || Boolean(dateTo) || Boolean(status);
  });

  constructor() {
    this.form.valueChanges
      .pipe(debounceTime(250), takeUntilDestroyed())
      .subscribe(() => this.filtersChange.emit(this.form.getRawValue()));
  }

  protected clear(): void {
    this.form.reset();
  }
}
