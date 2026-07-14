import { Component, input, output } from '@angular/core';
import { CartItemRowComponent } from '../cart-item-row/cart-item-row.component';
import { CartItemView } from '../models/cart-item-view.model';

export interface CartQuantityChange {
  productId: string;
  quantity: number;
}

@Component({
  selector: 'app-cart-panel',
  imports: [CartItemRowComponent],
  templateUrl: './cart-panel.component.html',
})
export class CartPanelComponent {
  readonly items = input.required<CartItemView[]>();
  readonly count = input.required<number>();
  readonly itemsTotalLabel = input.required<string>();

  readonly quantityChange = output<CartQuantityChange>();
  readonly remove = output<string>();
  readonly proceed = output<void>();
}
