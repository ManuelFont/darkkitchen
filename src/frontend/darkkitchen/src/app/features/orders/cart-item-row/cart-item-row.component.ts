import { Component, input, output } from '@angular/core';
import { CartItemView } from '../models/cart-item-view.model';

@Component({
  selector: 'app-cart-item-row',
  imports: [],
  templateUrl: './cart-item-row.component.html',
})
export class CartItemRowComponent {
  readonly item = input.required<CartItemView>();
  readonly quantityChange = output<number>();
  readonly remove = output<void>();

  protected decrease(): void {
    this.quantityChange.emit(this.item().quantity - 1);
  }

  protected increase(): void {
    this.quantityChange.emit(this.item().quantity + 1);
  }
}
