import { Component, computed, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OrderRow } from '../models/order-row.model';

@Component({
  selector: 'app-order-table',
  imports: [RouterLink],
  templateUrl: './order-table.component.html',
})
export class OrderTableComponent {
  readonly orders = input.required<OrderRow[]>();
  readonly loading = input.required<boolean>();
  readonly showCustomer = input(false);
  readonly manageable = input(false);

  readonly manage = output<OrderRow>();

  protected readonly skeletonRows = Array.from({ length: 5 });

  protected readonly columnCount = computed(
    () => 5 + (this.showCustomer() ? 1 : 0) + (this.manageable() ? 1 : 0),
  );
}
