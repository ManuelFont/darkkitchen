import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, map, of, startWith, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { formatMoney } from '../../../shared/utils/format-money';
import { formatMontevideoDateTime } from '../../../shared/utils/format-date';
import { OrderDetail } from '../models/order-detail.model';
import { OrderDetailView } from '../models/order-detail-view.model';
import { DEFAULT_STATUS_BADGE, STATUS_BADGES } from '../models/order-status';
import { OrdersService } from '../services/orders.service';

interface OrderDetailState {
  readonly loading: boolean;
  readonly order: OrderDetail | null;
  readonly failed: boolean;
}

const INITIAL_STATE: OrderDetailState = { loading: true, order: null, failed: false };

@Component({
  selector: 'app-order-detail',
  imports: [SidebarComponent, RouterLink],
  templateUrl: './order-detail.component.html',
})
export class OrderDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly ordersService = inject(OrdersService);
  private readonly toast = inject(ToastService);

  private readonly state = toSignal(
    this.route.params.pipe(
      map((params) => params['id'] as string),
      switchMap((id) =>
        this.ordersService.getById(id).pipe(
          map((order) => ({ loading: false, order, failed: false }) satisfies OrderDetailState),
          catchError((error: Error) => {
            this.toast.error(error.message);
            return of<OrderDetailState>({ loading: false, order: null, failed: true });
          }),
          startWith<OrderDetailState>({ loading: true, order: null, failed: false }),
        ),
      ),
    ),
    { initialValue: INITIAL_STATE },
  );

  protected readonly isLoading = computed(() => this.state().loading);
  protected readonly hasFailed = computed(() => this.state().failed);

  protected readonly view = computed<OrderDetailView | null>(() => {
    const order = this.state().order;
    if (!order) {
      return null;
    }

    const badge = STATUS_BADGES[order.status] ?? DEFAULT_STATUS_BADGE;

    return {
      shortId: order.orderId.slice(0, 8),
      createdAt: formatMontevideoDateTime(order.createdAt),
      statusLabel: badge.label,
      statusBadgeClass: badge.badgeClass,
      statusDotClass: badge.dotClass,
      deliveryTypeLabel: order.deliveryType === 'TwentyFourHours' ? '24 hours' : order.deliveryType,
      addressLine: this.formatAddress(order),
      items: order.items.map((item) => ({
        productName: item.productName,
        quantity: item.quantity,
        unitPriceLabel: formatMoney(item.unitPrice),
        subtotalLabel: formatMoney(item.subtotal),
      })),
      subtotalLabel: formatMoney(order.subtotal),
      deliveryLabel: formatMoney(order.deliveryCost),
      totalLabel: formatMoney(order.total),
    };
  });

  private formatAddress(order: OrderDetail): string {
    const base = `${order.street} ${order.doorNumber}`;
    return order.apartment ? `${base}, apt. ${order.apartment}` : base;
  }
}
