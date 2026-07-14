import { Component, computed, output } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { debounceTime, startWith } from 'rxjs';
import { DateRange } from '../models/date-range.model';

@Component({
  selector: 'app-date-range-filter',
  imports: [MatIconModule, ReactiveFormsModule],
  templateUrl: './date-range-filter.component.html',
})
export class DateRangeFilterComponent {
  readonly rangeChange = output<DateRange>();

  protected readonly form = new FormGroup({
    dateFrom: new FormControl('', { nonNullable: true }),
    dateTo: new FormControl('', { nonNullable: true }),
  });

  private readonly value = toSignal(
    this.form.valueChanges.pipe(startWith(this.form.getRawValue())),
    { initialValue: this.form.getRawValue() },
  );

  protected readonly hasValue = computed(() => {
    const { dateFrom, dateTo } = this.value();
    return Boolean(dateFrom) || Boolean(dateTo);
  });

  constructor() {
    this.form.valueChanges
      .pipe(debounceTime(250), takeUntilDestroyed())
      .subscribe(() => this.rangeChange.emit(this.form.getRawValue()));
  }

  protected clear(): void {
    this.form.reset();
  }
}
