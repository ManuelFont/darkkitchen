import { Component, input, output } from '@angular/core';
import { UserRow } from '../models/user-row.model';

@Component({
  selector: 'app-user-delete-modal',
  imports: [],
  templateUrl: './user-delete-modal.component.html',
})
export class UserDeleteModalComponent {
  readonly user = input.required<UserRow>();
  readonly isDeleting = input(false);

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();
}
