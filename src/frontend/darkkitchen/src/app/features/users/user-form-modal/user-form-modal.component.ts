import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, Observable } from 'rxjs';
import { CreateUserRequest } from '../models/create-user-request.model';
import { UpdateUserRequest } from '../models/update-user-request.model';
import { USER_ROLES } from '../models/user-role';
import { User } from '../models/user.model';
import { UsersService } from '../services/users.service';

@Component({
  selector: 'app-user-form-modal',
  imports: [FormsModule],
  templateUrl: './user-form-modal.component.html',
})
export class UserFormModalComponent implements OnInit {
  private readonly usersService = inject(UsersService);

  protected readonly roles = USER_ROLES;

  readonly user = input<User | null>(null);

  readonly closeRequested = output<void>();
  readonly saved = output<void>();

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly hasAttemptedSubmit = signal(false);

  protected readonly isEdit = computed(() => this.user() !== null);

  firstName = '';
  lastName = '';
  email = '';
  password = '';
  phone = '';
  roleId: number | null = null;

  ngOnInit(): void {
    const user = this.user();
    if (!user) {
      return;
    }

    this.firstName = user.firstName;
    this.lastName = user.lastName;
    this.email = user.email;
    this.phone = user.phone;
    this.roleId = user.roleId;
  }

  onSubmit(isFormValid: boolean | null): void {
    if (this.isSubmitting()) {
      return;
    }

    this.hasAttemptedSubmit.set(true);
    this.errorMessage.set(null);
    const roleId = this.roleId;

    if (!isFormValid || roleId === null) {
      this.errorMessage.set('Complete all required fields');
      return;
    }

    this.submit(roleId);
  }

  private submit(roleId: number): void {
    this.isSubmitting.set(true);

    this.persist(roleId)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => this.saved.emit(),
        error: (error: Error) => this.errorMessage.set(error.message),
      });
  }

  private persist(roleId: number): Observable<User> {
    const editedUser = this.user();

    return editedUser
      ? this.usersService.update(editedUser.id, this.buildUpdateRequest(roleId))
      : this.usersService.create(this.buildCreateRequest(roleId));
  }

  private buildCreateRequest(roleId: number): CreateUserRequest {
    return {
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      email: this.email.trim(),
      password: this.password,
      phone: this.phone.trim(),
      roleId,
    };
  }

  private buildUpdateRequest(roleId: number): UpdateUserRequest {
    const password = this.password.trim();

    return {
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      email: this.email.trim(),
      password: password ? password : undefined,
      phone: this.phone.trim(),
      roleId,
    };
  }
}
