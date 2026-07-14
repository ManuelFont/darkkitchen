import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { catchError, finalize, of, startWith, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { formatMoney } from '../../../shared/utils/format-money';
import { CategoryDropdownComponent } from '../../products/category-dropdown/category-dropdown.component';
import { ProductSearchComponent } from '../../products/product-search/product-search.component';
import { Product } from '../../products/models/product.model';
import type { ProductFilters } from '../../products/models/product-filters.model';
import { ProductsService } from '../../products/services/products.service';
import { CartPanelComponent, CartQuantityChange } from '../cart-panel/cart-panel.component';
import { OrderProductCardComponent } from '../order-product-card/order-product-card.component';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-new-order',
  imports: [
    SidebarComponent,
    RouterLink,
    ProductSearchComponent,
    CategoryDropdownComponent,
    OrderProductCardComponent,
    CartPanelComponent,
  ],
  templateUrl: './new-order.component.html',
})
export class NewOrderComponent {
  private readonly productsService = inject(ProductsService);
  private readonly cart = inject(CartService);
  private readonly router = inject(Router);

  protected readonly isLoading = signal(false);
  protected readonly selectedCategoryId = signal('');
  private readonly searchName = signal('');

  private readonly filters = computed<ProductFilters>(() => ({
    categoryId: this.selectedCategoryId(),
    name: this.searchName(),
  }));

  protected readonly products = toSignal(
    toObservable(this.filters).pipe(
      switchMap((filters) => {
        this.isLoading.set(true);

        return this.productsService.getAll(filters).pipe(
          catchError(() => of<Product[]>([])),
          finalize(() => this.isLoading.set(false)),
          startWith<Product[] | null>(null),
        );
      }),
    ),
    { initialValue: null },
  );

  protected readonly count = this.cart.count;
  protected readonly cartItems = this.cart.views;
  protected readonly itemsTotalLabel = computed(() => formatMoney(this.cart.itemsWithPromo()));

  protected searchProducts(name: string): void {
    this.selectedCategoryId.set('');
    this.searchName.set(name);
  }

  protected selectCategory(categoryId: string): void {
    this.searchName.set('');
    this.selectedCategoryId.set(categoryId);
  }

  protected addToCart(product: Product, quantity: number): void {
    this.cart.add(product, quantity);
  }

  protected changeQuantity(change: CartQuantityChange): void {
    this.cart.setQuantity(change.productId, change.quantity);
  }

  protected removeItem(productId: string): void {
    this.cart.remove(productId);
  }

  protected proceedToCheckout(): void {
    void this.router.navigateByUrl('/orders/checkout');
  }
}
