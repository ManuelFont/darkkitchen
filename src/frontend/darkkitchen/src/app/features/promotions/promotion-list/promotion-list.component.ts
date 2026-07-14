import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, combineLatest, finalize, map, of, startWith, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { formatMontevideoDateTime } from '../../../shared/utils/format-date';
import { ApplyProductsModalComponent } from '../apply-products-modal/apply-products-modal.component';
import { PromotionDeleteModalComponent } from '../promotion-delete-modal/promotion-delete-modal.component';
import { PromotionFiltersComponent } from '../promotion-filters/promotion-filters.component';
import { PromotionFormModalComponent } from '../promotion-form-modal/promotion-form-modal.component';
import { PromotionTableComponent } from '../promotion-table/promotion-table.component';
import { PromotionFilters } from '../models/promotion-filters.model';
import { PromotionRow } from '../models/promotion-row.model';
import { Promotion } from '../models/promotion.model';
import { PromotionsService } from '../services/promotions.service';

const TIME_ZONE_SUFFIX = /([zZ]|[+-]\d{2}:?\d{2})$/;

const STATUS_BADGES = {
  Active: { badgeClass: 'bg-success-500/15 text-success-300', dotClass: 'bg-success-400' },
  Scheduled: { badgeClass: 'bg-info-500/15 text-info-300', dotClass: 'bg-info-400' },
  Expired: { badgeClass: 'bg-accent-500/15 text-accent-300', dotClass: 'bg-accent-400' },
} as const;

type PromotionStatus = keyof typeof STATUS_BADGES;

interface PromotionsState {
  readonly loading: boolean;
  readonly promotions: Promotion[];
}

const INITIAL_STATE: PromotionsState = { loading: true, promotions: [] };
const NO_FILTERS: PromotionFilters = { search: '' };

@Component({
  selector: 'app-promotion-list',
  imports: [
    SidebarComponent,
    PromotionFiltersComponent,
    PromotionTableComponent,
    PromotionFormModalComponent,
    PromotionDeleteModalComponent,
    ApplyProductsModalComponent,
  ],
  templateUrl: './promotion-list.component.html',
})
export class PromotionListComponent {
  private readonly promotionsService = inject(PromotionsService);
  private readonly toast = inject(ToastService);

  private readonly reload = signal(0);
  private readonly filters = signal<PromotionFilters>(NO_FILTERS);

  protected readonly isFormOpen = signal(false);
  protected readonly editingPromotion = signal<Promotion | null>(null);
  protected readonly pendingDelete = signal<PromotionRow | null>(null);
  protected readonly isDeleting = signal(false);
  protected readonly applyingPromotion = signal<Promotion | null>(null);

  private readonly promotionsState = toSignal(
    combineLatest([toObservable(this.filters), toObservable(this.reload)]).pipe(
      switchMap(() =>
        this.promotionsService.getAll().pipe(
          map((promotions) => ({ loading: false, promotions }) satisfies PromotionsState),
          catchError(() => of<PromotionsState>({ loading: false, promotions: [] })),
          startWith<PromotionsState>({ loading: true, promotions: [] }),
        ),
      ),
    ),
    { initialValue: INITIAL_STATE },
  );

  protected readonly isLoading = computed(() => this.promotionsState().loading);

  protected readonly rows = computed<PromotionRow[]>(() => {
    const search = this.filters().search.trim().toLowerCase();
    return this.promotionsState()
      .promotions.filter((promotion) =>
        search ? promotion.name.toLowerCase().includes(search) : true,
      )
      .map((promotion) => this.toRow(promotion));
  });

  protected applyFilters(filters: PromotionFilters): void {
    this.filters.set(filters);
  }

  protected openCreateForm(): void {
    this.editingPromotion.set(null);
    this.isFormOpen.set(true);
  }

  protected openEditForm(row: PromotionRow): void {
    const promotion = this.findPromotion(row.id);
    if (!promotion) {
      return;
    }

    this.editingPromotion.set(promotion);
    this.isFormOpen.set(true);
  }

  protected closeForm(): void {
    this.isFormOpen.set(false);
  }

  protected onPromotionSaved(): void {
    const wasEditing = this.editingPromotion() !== null;
    this.closeForm();
    this.refresh();
    this.toast.success(wasEditing ? 'Promotion updated' : 'Promotion created');
  }

  protected requestDelete(promotion: PromotionRow): void {
    this.pendingDelete.set(promotion);
  }

  protected cancelDelete(): void {
    this.pendingDelete.set(null);
  }

  protected confirmDelete(): void {
    const promotion = this.pendingDelete();
    if (!promotion || this.isDeleting()) {
      return;
    }

    this.isDeleting.set(true);
    this.promotionsService
      .delete(promotion.id)
      .pipe(finalize(() => this.isDeleting.set(false)))
      .subscribe({
        next: () => {
          this.pendingDelete.set(null);
          this.refresh();
          this.toast.success('Promotion deleted');
        },
        error: (error: Error) => {
          this.pendingDelete.set(null);
          this.toast.error(error.message);
        },
      });
  }

  protected openApplyProducts(row: PromotionRow): void {
    const promotion = this.findPromotion(row.id);
    if (!promotion) {
      return;
    }

    this.applyingPromotion.set(promotion);
  }

  protected closeApplyProducts(): void {
    this.applyingPromotion.set(null);
  }

  protected onProductsSaved(): void {
    this.closeApplyProducts();
    this.refresh();
    this.toast.success('Products updated');
  }

  private refresh(): void {
    this.reload.update((value) => value + 1);
  }

  private findPromotion(id: string): Promotion | undefined {
    return this.promotionsState().promotions.find((candidate) => candidate.id === id);
  }

  private toRow(promotion: Promotion): PromotionRow {
    const status = this.resolveStatus(promotion);
    const badge = STATUS_BADGES[status];

    return {
      id: promotion.id,
      name: promotion.name,
      discountLabel: `${Math.round(promotion.discountPercentage * 100)}%`,
      startDate: formatMontevideoDateTime(promotion.startDate),
      endDate: formatMontevideoDateTime(promotion.endDate),
      statusLabel: status,
      statusBadgeClass: badge.badgeClass,
      statusDotClass: badge.dotClass,
    };
  }

  private resolveStatus(promotion: Promotion): PromotionStatus {
    const now = Date.now();
    const start = this.parseDate(promotion.startDate);
    const end = this.parseDate(promotion.endDate);

    if (now < start) {
      return 'Scheduled';
    }

    if (now > end) {
      return 'Expired';
    }

    return 'Active';
  }

  private parseDate(value: string): number {
    const normalized = TIME_ZONE_SUFFIX.test(value) ? value : `${value}Z`;
    return new Date(normalized).getTime();
  }
}
