import { Component, computed, input, output } from '@angular/core';
import { OrderAction, OrderActionDescriptor } from '../models/order-action';
import { OrderRow } from '../models/order-row.model';

interface OrderActionButton {
  readonly action: OrderAction;
  readonly label: string;
  readonly buttonClass: string;
}

const PRIMARY_BUTTON_CLASS =
  'h-12 rounded-lg bg-accent px-6 text-base font-semibold text-white shadow-sm transition hover:cursor-pointer hover:bg-accent-600 disabled:cursor-not-allowed disabled:opacity-60';
const DANGER_BUTTON_CLASS =
  'h-12 rounded-lg bg-red-700 px-6 text-base font-semibold text-white shadow-sm transition hover:cursor-pointer hover:bg-red-800 disabled:cursor-not-allowed disabled:opacity-60';

@Component({
  selector: 'app-order-status-modal',
  imports: [],
  templateUrl: './order-status-modal.component.html',
})
export class OrderStatusModalComponent {
  readonly order = input.required<OrderRow>();
  readonly actions = input.required<readonly OrderActionDescriptor[]>();
  readonly isSubmitting = input(false);
  readonly pendingAction = input<OrderAction | null>(null);

  readonly action = output<OrderAction>();
  readonly cancelled = output<void>();

  protected readonly buttons = computed<OrderActionButton[]>(() =>
    this.actions().map((descriptor) => ({
      action: descriptor.action,
      label: descriptor.label,
      buttonClass: descriptor.variant === 'danger' ? DANGER_BUTTON_CLASS : PRIMARY_BUTTON_CLASS,
    })),
  );
}
