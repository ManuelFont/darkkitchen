import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, combineLatest, finalize, map, of, startWith, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { UserDeleteModalComponent } from '../user-delete-modal/user-delete-modal.component';
import { UserFiltersComponent } from '../user-filters/user-filters.component';
import { UserFormModalComponent } from '../user-form-modal/user-form-modal.component';
import { UserTableComponent } from '../user-table/user-table.component';
import { UserFilters } from '../models/user-filters.model';
import { UserRow } from '../models/user-row.model';
import { User } from '../models/user.model';
import { UsersService } from '../services/users.service';

const ROLE_BADGES: Record<string, { readonly badgeClass: string; readonly dotClass: string }> = {
  Customer: { badgeClass: 'bg-info-500/15 text-info-300', dotClass: 'bg-info-400' },
  Dispatcher: { badgeClass: 'bg-accent-500/15 text-accent-300', dotClass: 'bg-accent-400' },
  Administrator: { badgeClass: 'bg-success-500/15 text-success-300', dotClass: 'bg-success-400' },
};

const DEFAULT_ROLE_BADGE = {
  badgeClass: 'bg-stone-500/15 text-stone-300',
  dotClass: 'bg-stone-400',
};

interface UsersState {
  readonly loading: boolean;
  readonly users: User[];
}

const INITIAL_USERS_STATE: UsersState = { loading: true, users: [] };
const NO_FILTERS: UserFilters = { search: '', role: '' };

@Component({
  selector: 'app-user-list',
  imports: [
    SidebarComponent,
    UserFiltersComponent,
    UserTableComponent,
    UserFormModalComponent,
    UserDeleteModalComponent,
  ],
  templateUrl: './user-list.component.html',
})
export class UserListComponent {
  private readonly usersService = inject(UsersService);
  private readonly toast = inject(ToastService);

  private readonly reload = signal(0);
  private readonly filters = signal<UserFilters>(NO_FILTERS);

  protected readonly isFormOpen = signal(false);
  protected readonly editingUser = signal<User | null>(null);
  protected readonly pendingDelete = signal<UserRow | null>(null);
  protected readonly isDeleting = signal(false);

  private readonly usersState = toSignal(
    combineLatest([toObservable(this.filters), toObservable(this.reload)]).pipe(
      switchMap(([filters]) =>
        this.usersService.getAll(filters).pipe(
          map((users) => ({ loading: false, users }) satisfies UsersState),
          catchError(() => of<UsersState>({ loading: false, users: [] })),
          startWith<UsersState>({ loading: true, users: [] }),
        ),
      ),
    ),
    { initialValue: INITIAL_USERS_STATE },
  );

  protected readonly isLoading = computed(() => this.usersState().loading);

  protected readonly rows = computed<UserRow[]>(() =>
    this.usersState().users.map((user) => this.toRow(user)),
  );

  protected applyFilters(filters: UserFilters): void {
    this.filters.set(filters);
  }

  protected openCreateForm(): void {
    this.editingUser.set(null);
    this.isFormOpen.set(true);
  }

  protected openEditForm(row: UserRow): void {
    const user = this.usersState().users.find((candidate) => candidate.id === row.id);
    if (!user) {
      return;
    }

    this.editingUser.set(user);
    this.isFormOpen.set(true);
  }

  protected closeForm(): void {
    this.isFormOpen.set(false);
  }

  protected onUserSaved(): void {
    const wasEditing = this.editingUser() !== null;
    this.closeForm();
    this.refresh();
    this.toast.success(wasEditing ? 'User updated' : 'User created');
  }

  protected requestDelete(user: UserRow): void {
    this.pendingDelete.set(user);
  }

  protected cancelDelete(): void {
    this.pendingDelete.set(null);
  }

  protected confirmDelete(): void {
    const user = this.pendingDelete();
    if (!user || this.isDeleting()) {
      return;
    }

    this.isDeleting.set(true);
    this.usersService
      .delete(user.id)
      .pipe(finalize(() => this.isDeleting.set(false)))
      .subscribe({
        next: () => {
          this.pendingDelete.set(null);
          this.refresh();
          this.toast.success('User deleted');
        },
        error: (error: Error) => {
          this.pendingDelete.set(null);
          this.toast.error(error.message);
        },
      });
  }

  private refresh(): void {
    this.reload.update((value) => value + 1);
  }

  private toRow(user: User): UserRow {
    const badge = ROLE_BADGES[user.roleName] ?? DEFAULT_ROLE_BADGE;

    return {
      id: user.id,
      fullName: `${user.firstName} ${user.lastName}`,
      email: user.email,
      phone: user.phone,
      roleName: user.roleName,
      roleBadgeClass: badge.badgeClass,
      roleDotClass: badge.dotClass,
    };
  }
}
