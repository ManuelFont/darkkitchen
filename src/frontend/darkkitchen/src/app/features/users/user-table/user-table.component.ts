import { SlicePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { UserRow } from '../models/user-row.model';

@Component({
  selector: 'app-user-table',
  imports: [SlicePipe, MatIconModule],
  templateUrl: './user-table.component.html',
})
export class UserTableComponent {
  readonly users = input.required<UserRow[]>();
  readonly loading = input(false);

  readonly editUser = output<UserRow>();
  readonly deleteUser = output<UserRow>();

  protected readonly skeletonRows = Array.from({ length: 6 });
}
