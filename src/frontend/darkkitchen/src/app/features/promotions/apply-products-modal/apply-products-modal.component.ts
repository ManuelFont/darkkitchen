import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { finalize, forkJoin, Observable } from 'rxjs';
import { ToastService } from '../../../core/toast/toast.service';
import { ProductsService } from '../../products/services/products.service';
import { Promotion } from '../models/promotion.model';

interface ProductOption {
  readonly id: string;
  readonly name: string;
}

@Component({
  selector: 'app-apply-products-modal',
  imports: [],
  templateUrl: './apply-products-modal.component.html',
})
export class ApplyProductsModalComponent implements OnInit {
  private readonly productsService = inject(ProductsService);
  private readonly toast = inject(ToastService);

  readonly promotion = input.required<Promotion>();

  readonly closeRequested = output<void>();
  readonly saved = output<void>();

  protected readonly isLoading = signal(true);
  protected readonly hasLoadError = signal(false);
  protected readonly isSubmitting = signal(false);
  protected readonly products = signal<ProductOption[]>([]);

  private readonly initialSelected = signal<ReadonlySet<string>>(new Set());
  protected readonly selected = signal<ReadonlySet<string>>(new Set());

  protected readonly isEmpty = computed(
    () => !this.isLoading() && !this.hasLoadError() && this.products().length === 0,
  );

  ngOnInit(): void {
    const promotionId = this.promotion().id;

    this.productsService.getAll().subscribe({
      next: (products) => {
        this.products.set(products.map((product) => ({ id: product.id, name: product.name })));
        const applied = new Set(
          products
            .filter((product) => product.promotions.some((promo) => promo.id === promotionId))
            .map((product) => product.id),
        );
        this.initialSelected.set(applied);
        this.selected.set(new Set(applied));
        this.isLoading.set(false);
      },
      error: (error: Error) => {
        this.isLoading.set(false);
        this.hasLoadError.set(true);
        this.toast.error(error.message);
      },
    });
  }

  protected isSelected(productId: string): boolean {
    return this.selected().has(productId);
  }

  protected toggle(productId: string): void {
    this.selected.update((current) => {
      const next = new Set(current);
      if (next.has(productId)) {
        next.delete(productId);
      } else {
        next.add(productId);
      }
      return next;
    });
  }

  protected onSubmit(): void {
    if (this.isSubmitting() || this.isLoading()) {
      return;
    }

    const promotionId = this.promotion().id;
    const initial = this.initialSelected();
    const selected = this.selected();

    const toApply = [...selected].filter((id) => !initial.has(id));
    const toRemove = [...initial].filter((id) => !selected.has(id));

    if (toApply.length === 0 && toRemove.length === 0) {
      this.closeRequested.emit();
      return;
    }

    const calls: Observable<void>[] = [
      ...toApply.map((productId) => this.productsService.applyPromotion(productId, promotionId)),
      ...toRemove.map((productId) => this.productsService.removePromotion(productId, promotionId)),
    ];

    this.isSubmitting.set(true);
    forkJoin(calls)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => this.saved.emit(),
        error: (error: Error) => this.toast.error(error.message),
      });
  }
}
