import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, combineLatest, finalize, map, of, startWith, switchMap } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { Roles } from '../../../core/auth/role.model';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { formatMoney } from '../../../shared/utils/format-money';
import { formatMontevideoDateTime } from '../../../shared/utils/format-date';
import { OrderFiltersComponent } from '../order-filters/order-filters.component';
import { OrderStatusModalComponent } from '../order-status-modal/order-status-modal.component';
import { OrderTableComponent } from '../order-table/order-table.component';
import { availableActions, OrderAction } from '../models/order-action';
import { OrderFilters } from '../models/order-filters.model';
import { OrderListItem } from '../models/order-list-item.model';
import { OrderRow } from '../models/order-row.model';
import { DEFAULT_STATUS_BADGE, STATUS_BADGES } from '../models/order-status';
import { ClientOrderSummary } from '../models/order-summary.model';
import { OrdersService } from '../services/orders.service';

const NO_FILTERS: OrderFilters = { dateFrom: '', dateTo: '', status: '' };

interface OrdersState {
  readonly loading: boolean;
  readonly rows: OrderRow[];
}

const INITIAL_STATE: OrdersState = { loading: true, rows: [] };

@Component({
  selector: 'app-order-history',
  imports: [
    SidebarComponent,
    RouterLink,
    OrderFiltersComponent,
    OrderTableComponent,
    OrderStatusModalComponent,
  ],
  templateUrl: './order-history.component.html',
})
export class OrderHistoryComponent {
  private readonly ordersService = inject(OrdersService);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);

  protected readonly isManager = this.auth.hasRole(Roles.Dispatcher, Roles.Administrator);

  private readonly filters = signal<OrderFilters>(NO_FILTERS);
  private readonly reload = signal(0);

  protected readonly managingOrder = signal<OrderRow | null>(null);
  protected readonly isSubmitting = signal(false);
  protected readonly pendingAction = signal<OrderAction | null>(null);

  protected readonly managingActions = computed(() => {
    const order = this.managingOrder();
    return order ? availableActions(order.status, this.auth.currentRole()) : [];
  });

  private readonly state = toSignal(
    combineLatest([toObservable(this.filters), toObservable(this.reload)]).pipe(
      switchMap(([filters]) =>
        this.fetchRows(filters).pipe(
          map((rows) => ({ loading: false, rows }) satisfies OrdersState),
          catchError((error: Error) => {
            this.toast.error(error.message);
            return of<OrdersState>({ loading: false, rows: [] });
          }),
          startWith<OrdersState>({ loading: true, rows: [] }),
        ),
      ),
    ),
    { initialValue: INITIAL_STATE },
  );

  protected readonly isLoading = computed(() => this.state().loading);
  protected readonly isEmpty = computed(
    () => !this.state().loading && this.state().rows.length === 0,
  );
  protected readonly rows = computed<OrderRow[]>(() => this.state().rows);

  protected applyFilters(filters: OrderFilters): void {
    this.filters.set(filters);
  }

  protected openManage(row: OrderRow): void {
    this.managingOrder.set(row);
  }

  protected closeManage(): void {
    if (this.isSubmitting()) {
      return;
    }
    this.managingOrder.set(null);
  }

  protected changeStatus(action: OrderAction): void {
    const order = this.managingOrder();
    if (!order || this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);
    this.pendingAction.set(action);
    this.ordersService
      .changeStatus(order.orderId, action)
      .pipe(
        finalize(() => {
          this.isSubmitting.set(false);
          this.pendingAction.set(null);
        }),
      )
      .subscribe({
        next: () => {
          this.managingOrder.set(null);
          this.refresh();
          this.toast.success('Order updated');
        },
        error: (error: Error) => this.toast.error(error.message),
      });
  }

  private refresh(): void {
    this.reload.update((value) => value + 1);
  }

  private fetchRows(filters: OrderFilters) {
    if (this.isManager) {
      return this.ordersService
        .getAllOrders(filters)
        .pipe(map((orders) => orders.map((order) => this.toManagerRow(order))));
    }

    return this.ordersService
      .getMyOrders(filters)
      .pipe(map((orders) => orders.map((order) => this.toCustomerRow(order))));
  }

  private toCustomerRow(order: ClientOrderSummary): OrderRow {
    const badge = STATUS_BADGES[order.status] ?? DEFAULT_STATUS_BADGE;

    return {
      orderId: order.orderId,
      shortId: order.orderId.slice(0, 8),
      createdAt: formatMontevideoDateTime(order.createdAt),
      productCount: order.productCount,
      total: formatMoney(order.total),
      statusLabel: badge.label,
      statusBadgeClass: badge.badgeClass,
      statusDotClass: badge.dotClass,
      status: order.status,
    };
  }

  private toManagerRow(order: OrderListItem): OrderRow {
    const badge = STATUS_BADGES[order.status] ?? DEFAULT_STATUS_BADGE;
    const productCount = order.items.reduce((sum, item) => sum + item.quantity, 0);

    return {
      orderId: order.orderId,
      shortId: order.orderId.slice(0, 8),
      createdAt: formatMontevideoDateTime(order.createdAt),
      productCount,
      total: formatMoney(order.total),
      statusLabel: badge.label,
      statusBadgeClass: badge.badgeClass,
      statusDotClass: badge.dotClass,
      status: order.status,
      customerName: `${order.clientFirstName} ${order.clientLastName}`.trim(),
      customerEmail: order.clientEmail,
      hasActions: availableActions(order.status, this.auth.currentRole()).length > 0,
    };
  }
}
