import { NgTemplateOutlet } from '@angular/common';
import { Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { formatMoney } from '../../../shared/utils/format-money';
import { Product } from '../models/product.model';

@Component({
  selector: 'app-product-card',
  imports: [NgTemplateOutlet, RouterLink],
  templateUrl: './product-card.component.html',
})
export class ProductCardComponent {
  readonly product = input.required<Product>();
  readonly canOpenProduct = input(true);
  readonly cardClass =
    'group block w-full overflow-hidden rounded-xl border border-stone-200 bg-white text-inherit shadow-sm transition duration-300';

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
}
