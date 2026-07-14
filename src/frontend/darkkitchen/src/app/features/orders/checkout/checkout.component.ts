import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { formatMoney } from '../../../shared/utils/format-money';
import { AddressFormComponent } from '../address-form/address-form.component';
import { DeliveryOptionSelectorComponent } from '../delivery-option-selector/delivery-option-selector.component';
import { OrderReviewComponent } from '../order-review/order-review.component';
import { AddressFormChange } from '../models/address-form-value.model';
import { CreateOrderRequest } from '../models/create-order-request.model';
import { DeliveryType } from '../../delivery-types/models/delivery-type.model';
import { DeliveryTypesService } from '../../delivery-types/services/delivery-types.service';
import { CartService } from '../services/cart.service';
import { OrdersService } from '../services/orders.service';

@Component({
  selector: 'app-checkout',
  imports: [
    SidebarComponent,
    RouterLink,
    DeliveryOptionSelectorComponent,
    AddressFormComponent,
    OrderReviewComponent,
  ],
  templateUrl: './checkout.component.html',
})
export class CheckoutComponent {
  private readonly cart = inject(CartService);
  private readonly ordersService = inject(OrdersService);
  private readonly deliveryTypesService = inject(DeliveryTypesService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly deliveryTypes = signal<DeliveryType[]>([]);
  protected readonly selectedDeliveryId = signal('');
  protected readonly isSubmitting = signal(false);
  protected readonly showAddressErrors = signal(false);
  private readonly address = signal<AddressFormChange | null>(null);

  protected readonly cartItems = this.cart.views;
  protected readonly count = this.cart.count;

  private readonly selectedType = computed(
    () =>
      this.deliveryTypes().find((candidate) => candidate.id === this.selectedDeliveryId()) ?? null,
  );

  private readonly estimate = computed(() => this.cart.estimate(this.selectedType()?.cost ?? 0));

  protected readonly subtotalLabel = computed(() => formatMoney(this.estimate().itemsWithPromo));
  protected readonly deliveryLabel = computed(() => formatMoney(this.estimate().deliveryCost));
  protected readonly totalLabel = computed(() => formatMoney(this.estimate().total));

  constructor() {
    if (this.cart.count() === 0) {
      void this.router.navigateByUrl('/orders/new');
      return;
    }

    this.deliveryTypesService.getAll().subscribe({
      next: (types) => {
        this.deliveryTypes.set(types);
        if (types.length > 0 && !this.selectedDeliveryId()) {
          this.selectedDeliveryId.set(types[0].id);
        }
      },
      error: (error: Error) => this.toast.error(error.message),
    });
  }

  protected selectDelivery(id: string): void {
    this.selectedDeliveryId.set(id);
  }

  protected onAddressChange(change: AddressFormChange): void {
    this.address.set(change);
  }

  protected placeOrder(): void {
    if (this.isSubmitting()) {
      return;
    }

    const address = this.address();
    if (!address || !address.valid || address.value.doorNumber === null) {
      this.showAddressErrors.set(true);
      return;
    }

    const items = this.cart.items();
    if (items.length === 0) {
      void this.router.navigateByUrl('/orders/new');
      return;
    }

    const deliveryTypeId = this.selectedDeliveryId();
    if (!deliveryTypeId) {
      this.toast.error('Select a delivery option');
      return;
    }

    const request: CreateOrderRequest = {
      deliveryTypeId,
      address: {
        street: address.value.street,
        doorNumber: address.value.doorNumber,
        apartment: address.value.apartment ? address.value.apartment : null,
      },
      items: items.map((item) => ({ productId: item.productId, quantity: item.quantity })),
    };

    this.isSubmitting.set(true);
    this.ordersService
      .create(request)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (response) => {
          this.cart.clear();
          this.toast.success('Order placed');
          void this.router.navigateByUrl(`/orders/${response.orderId}`);
        },
        error: (error: Error) => {
          this.toast.error(error.message);
        },
      });
  }
}
