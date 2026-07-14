import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, Observable } from 'rxjs';
import { ToastService } from '../../../core/toast/toast.service';
import { DeliveryType } from '../models/delivery-type.model';
import { DeliveryTypeRequest } from '../models/delivery-type-request.model';
import { DeliveryTypesService } from '../services/delivery-types.service';

@Component({
  selector: 'app-delivery-type-form-modal',
  imports: [FormsModule],
  templateUrl: './delivery-type-form-modal.component.html',
})
export class DeliveryTypeFormModalComponent implements OnInit {
  private readonly deliveryTypesService = inject(DeliveryTypesService);
  private readonly toast = inject(ToastService);

  readonly deliveryType = input<DeliveryType | null>(null);

  readonly closeRequested = output<void>();
  readonly saved = output<void>();

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly hasAttemptedSubmit = signal(false);

  protected readonly isEdit = computed(() => this.deliveryType() !== null);

  name = '';
  cost: number | null = null;

  ngOnInit(): void {
    const deliveryType = this.deliveryType();
    if (!deliveryType) {
      return;
    }

    this.name = deliveryType.name;
    this.cost = deliveryType.cost;
  }

  onSubmit(isFormValid: boolean | null): void {
    if (this.isSubmitting()) {
      return;
    }

    this.hasAttemptedSubmit.set(true);
    this.errorMessage.set(null);
    const cost = this.cost;

    if (!isFormValid || cost === null) {
      this.errorMessage.set('Complete all required fields');
      return;
    }

    this.submit(cost);
  }

  private submit(cost: number): void {
    this.isSubmitting.set(true);

    this.persist(cost)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => this.saved.emit(),
        error: (error: Error) => this.toast.error(error.message),
      });
  }

  private persist(cost: number): Observable<DeliveryType | void> {
    const request = this.buildRequest(cost);
    const editedDeliveryType = this.deliveryType();

    return editedDeliveryType
      ? this.deliveryTypesService.update(editedDeliveryType.id, request)
      : this.deliveryTypesService.create(request);
  }

  private buildRequest(cost: number): DeliveryTypeRequest {
    return {
      name: this.name.trim(),
      cost,
    };
  }
}
