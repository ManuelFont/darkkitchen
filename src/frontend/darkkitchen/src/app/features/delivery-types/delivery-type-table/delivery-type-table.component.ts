import { SlicePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { DeliveryTypeRow } from '../models/delivery-type-row.model';

@Component({
  selector: 'app-delivery-type-table',
  imports: [SlicePipe, MatIconModule],
  templateUrl: './delivery-type-table.component.html',
})
export class DeliveryTypeTableComponent {
  readonly deliveryTypes = input.required<DeliveryTypeRow[]>();
  readonly loading = input(false);

  readonly edit = output<DeliveryTypeRow>();

  protected readonly skeletonRows = Array.from({ length: 4 });
}
