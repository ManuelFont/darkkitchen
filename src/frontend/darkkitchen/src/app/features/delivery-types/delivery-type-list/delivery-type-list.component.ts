import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of, startWith, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { formatMoney } from '../../../shared/utils/format-money';
import { DeliveryTypeFormModalComponent } from '../delivery-type-form-modal/delivery-type-form-modal.component';
import { DeliveryTypeTableComponent } from '../delivery-type-table/delivery-type-table.component';
import { DeliveryType } from '../models/delivery-type.model';
import { DeliveryTypeRow } from '../models/delivery-type-row.model';
import { DeliveryTypesService } from '../services/delivery-types.service';

interface DeliveryTypesState {
  readonly loading: boolean;
  readonly deliveryTypes: DeliveryType[];
}

const INITIAL_STATE: DeliveryTypesState = { loading: true, deliveryTypes: [] };

@Component({
  selector: 'app-delivery-type-list',
  imports: [SidebarComponent, DeliveryTypeTableComponent, DeliveryTypeFormModalComponent],
  templateUrl: './delivery-type-list.component.html',
})
export class DeliveryTypeListComponent {
  private readonly deliveryTypesService = inject(DeliveryTypesService);
  private readonly toast = inject(ToastService);

  private readonly reload = signal(0);

  protected readonly isFormOpen = signal(false);
  protected readonly editingDeliveryType = signal<DeliveryType | null>(null);

  private readonly deliveryTypesState = toSignal(
    toObservable(this.reload).pipe(
      switchMap(() =>
        this.deliveryTypesService.getAll().pipe(
          map((deliveryTypes) => ({ loading: false, deliveryTypes }) satisfies DeliveryTypesState),
          catchError(() => of<DeliveryTypesState>({ loading: false, deliveryTypes: [] })),
          startWith<DeliveryTypesState>({ loading: true, deliveryTypes: [] }),
        ),
      ),
    ),
    { initialValue: INITIAL_STATE },
  );

  protected readonly isLoading = computed(() => this.deliveryTypesState().loading);

  protected readonly rows = computed<DeliveryTypeRow[]>(() =>
    this.deliveryTypesState().deliveryTypes.map((deliveryType) => this.toRow(deliveryType)),
  );

  protected openCreateForm(): void {
    this.editingDeliveryType.set(null);
    this.isFormOpen.set(true);
  }

  protected openEditForm(row: DeliveryTypeRow): void {
    const deliveryType = this.deliveryTypesState().deliveryTypes.find(
      (candidate) => candidate.id === row.id,
    );
    if (!deliveryType) {
      return;
    }

    this.editingDeliveryType.set(deliveryType);
    this.isFormOpen.set(true);
  }

  protected closeForm(): void {
    this.isFormOpen.set(false);
  }

  protected onDeliveryTypeSaved(): void {
    const wasEditing = this.editingDeliveryType() !== null;
    this.closeForm();
    this.refresh();
    this.toast.success(wasEditing ? 'Delivery type updated' : 'Delivery type created');
  }

  private refresh(): void {
    this.reload.update((value) => value + 1);
  }

  private toRow(deliveryType: DeliveryType): DeliveryTypeRow {
    return {
      id: deliveryType.id,
      name: deliveryType.name,
      costLabel: formatMoney(deliveryType.cost),
    };
  }
}
