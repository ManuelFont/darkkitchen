import { computed, effect, Injectable, signal } from '@angular/core';
import { formatMoney } from '../../../shared/utils/format-money';
import { Product } from '../../products/models/product.model';
import { CartItem } from '../models/cart-item.model';
import { CartItemView } from '../models/cart-item-view.model';

const CART_STORAGE_KEY = 'darkkitchen.cart';
const IVA_RATE = 1.22;

export interface CartEstimate {
  itemsWithPromo: number;
  deliveryCost: number;
  total: number;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private readonly cart = signal<CartItem[]>(this.readFromStorage());

  public readonly items = this.cart.asReadonly();

  public readonly count = computed(() =>
    this.cart().reduce((total, item) => total + item.quantity, 0),
  );

  public readonly itemsWithPromo = computed(() =>
    this.cart().reduce((total, item) => total + this.lineWithPromo(item), 0),
  );

  public readonly views = computed<CartItemView[]>(() =>
    this.cart().map((item) => ({
      productId: item.productId,
      name: item.name,
      imageUrl: item.imageUrl,
      quantity: item.quantity,
      unitPriceLabel: formatMoney(item.unitPrice),
      lineLabel: formatMoney(this.lineWithPromo(item)),
      originalLineLabel: item.discount > 0 ? formatMoney(item.unitPrice * item.quantity) : null,
    })),
  );

  constructor() {
    effect(() => {
      localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(this.cart()));
    });
  }

  public add(product: Product, quantity = 1): void {
    if (quantity < 1) {
      return;
    }

    this.cart.update((items) => {
      const existing = items.find((item) => item.productId === product.id);
      if (existing) {
        return items.map((item) =>
          item.productId === product.id ? { ...item, quantity: item.quantity + quantity } : item,
        );
      }

      return [...items, this.toCartItem(product, quantity)];
    });
  }

  public setQuantity(productId: string, quantity: number): void {
    if (quantity < 1) {
      this.remove(productId);
      return;
    }

    this.cart.update((items) =>
      items.map((item) => (item.productId === productId ? { ...item, quantity } : item)),
    );
  }

  public remove(productId: string): void {
    this.cart.update((items) => items.filter((item) => item.productId !== productId));
  }

  public clear(): void {
    this.cart.set([]);
  }

  public estimate(deliveryCost: number): CartEstimate {
    const itemsWithPromo = this.itemsWithPromo();
    const total = (itemsWithPromo + deliveryCost) * IVA_RATE;

    return { itemsWithPromo, deliveryCost, total };
  }

  private lineWithPromo(item: CartItem): number {
    const unit = item.discount > 0 ? item.unitPrice * (1 - item.discount) : item.unitPrice;
    return unit * item.quantity;
  }

  private toCartItem(product: Product, quantity: number): CartItem {
    return {
      productId: product.id,
      name: product.name,
      imageUrl: product.imagesUrls[0] ?? '',
      unitPrice: product.price,
      discount: product.activePromotion?.discountPercentage ?? 0,
      quantity,
    };
  }

  private readFromStorage(): CartItem[] {
    const raw = localStorage.getItem(CART_STORAGE_KEY);
    if (!raw) {
      return [];
    }

    try {
      const parsed = JSON.parse(raw) as CartItem[];
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }
}
