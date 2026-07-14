import { Component, input } from '@angular/core';
import { CartItemView } from '../models/cart-item-view.model';

@Component({
  selector: 'app-order-review',
  imports: [],
  templateUrl: './order-review.component.html',
})
export class OrderReviewComponent {
  readonly items = input.required<CartItemView[]>();
  readonly subtotalLabel = input.required<string>();
  readonly deliveryLabel = input.required<string>();
  readonly totalLabel = input.required<string>();
}
