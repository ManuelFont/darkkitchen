import { Component, computed, input, output, signal } from '@angular/core';
import { Product } from '../../products/models/product.model';
import { formatMoney } from '../../../shared/utils/format-money';

@Component({
  selector: 'app-order-product-card',
  imports: [],
  templateUrl: './order-product-card.component.html',
})
export class OrderProductCardComponent {
  readonly product = input.required<Product>();
  readonly addToCart = output<number>();

  protected readonly quantity = signal(1);

  protected readonly discountLabel = computed(() => {
    const promotion = this.product().activePromotion;
    return promotion ? `${Math.round(promotion.discountPercentage * 100)}% OFF` : null;
  });

  protected readonly priceLabel = computed(() => formatMoney(this.product().price));

  protected readonly promoPriceLabel = computed(() => {
    const promotion = this.product().activePromotion;
    if (!promotion) {
      return null;
    }

    return formatMoney(this.product().price * (1 - promotion.discountPercentage));
  });

  protected decrease(): void {
    this.quantity.update((value) => Math.max(1, value - 1));
  }

  protected increase(): void {
    this.quantity.update((value) => value + 1);
  }

  protected add(): void {
    this.addToCart.emit(this.quantity());
    this.quantity.set(1);
  }
}
