import { Component, output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { USER_ROLES } from '../models/user-role';
import { UserFilters } from '../models/user-filters.model';

@Component({
  selector: 'app-user-filters',
  imports: [ReactiveFormsModule],
  templateUrl: './user-filters.component.html',
})
export class UserFiltersComponent {
  protected readonly roles = USER_ROLES;

  readonly filtersChange = output<UserFilters>();

  protected readonly form = new FormGroup({
    search: new FormControl('', { nonNullable: true }),
    role: new FormControl('', { nonNullable: true }),
  });

  constructor() {
    this.form.valueChanges
      .pipe(debounceTime(250), takeUntilDestroyed())
      .subscribe(() => this.filtersChange.emit(this.form.getRawValue()));
  }
}
