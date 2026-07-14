import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, Observable } from 'rxjs';
import { ToastService } from '../../../core/toast/toast.service';
import { Promotion } from '../models/promotion.model';
import { PromotionRequest } from '../models/promotion-request.model';
import { PromotionsService } from '../services/promotions.service';

export const NAME_PATTERN = /^[A-Za-z\s]+$/;

@Component({
  selector: 'app-promotion-form-modal',
  imports: [FormsModule],
  templateUrl: './promotion-form-modal.component.html',
})
export class PromotionFormModalComponent implements OnInit {
  private readonly promotionsService = inject(PromotionsService);
  private readonly toast = inject(ToastService);

  protected readonly namePattern = NAME_PATTERN;

  readonly promotion = input<Promotion | null>(null);

  readonly closeRequested = output<void>();
  readonly saved = output<void>();

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly hasAttemptedSubmit = signal(false);

  protected readonly isEdit = computed(() => this.promotion() !== null);

  name = '';
  discountPercentage: number | null = null;
  startDate = '';
  endDate = '';

  ngOnInit(): void {
    const promotion = this.promotion();
    if (!promotion) {
      return;
    }

    this.name = promotion.name;
    this.discountPercentage = Math.round(promotion.discountPercentage * 100);
    this.startDate = promotion.startDate.slice(0, 10);
    this.endDate = promotion.endDate.slice(0, 10);
  }

  protected get isEndDateBeforeStartDate(): boolean {
    return !!this.startDate && !!this.endDate && this.endDate < this.startDate;
  }

  onSubmit(isFormValid: boolean | null): void {
    if (this.isSubmitting()) {
      return;
    }

    this.hasAttemptedSubmit.set(true);
    this.errorMessage.set(null);
    const percentage = this.discountPercentage;

    if (!isFormValid || percentage === null || this.isEndDateBeforeStartDate) {
      this.errorMessage.set('Complete all required fields');
      return;
    }

    this.submit(percentage);
  }

  private submit(percentage: number): void {
    this.isSubmitting.set(true);

    this.persist(percentage)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => this.saved.emit(),
        error: (error: Error) => this.toast.error(error.message),
      });
  }

  private persist(percentage: number): Observable<Promotion> {
    const request = this.buildRequest(percentage);
    const editedPromotion = this.promotion();

    return editedPromotion
      ? this.promotionsService.update(editedPromotion.id, request)
      : this.promotionsService.create(request);
  }

  private buildRequest(percentage: number): PromotionRequest {
    return {
      name: this.name.trim(),
      discountPercentage: percentage / 100,
      startDate: this.startDate,
      endDate: this.endDate,
    };
  }
}
