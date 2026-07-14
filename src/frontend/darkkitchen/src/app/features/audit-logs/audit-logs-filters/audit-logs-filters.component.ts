import { Component, output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { AuditLogFilters } from '../models/audit-log-filters.model';

@Component({
  selector: 'app-audit-logs-filters',
  imports: [ReactiveFormsModule],
  templateUrl: './audit-logs-filters.component.html',
})
export class AuditLogsFiltersComponent {
  readonly filtersChange = output<AuditLogFilters>();

  protected readonly form = new FormGroup({
    entityName: new FormControl('', { nonNullable: true }),
    userEmail: new FormControl('', { nonNullable: true }),
    action: new FormControl('', { nonNullable: true }),
  });

  constructor() {
    this.form.valueChanges
      .pipe(debounceTime(300), takeUntilDestroyed())
      .subscribe(() => this.filtersChange.emit(this.form.getRawValue()));
  }
}
