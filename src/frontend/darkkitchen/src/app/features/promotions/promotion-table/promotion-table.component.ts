import { SlicePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { PromotionRow } from '../models/promotion-row.model';

@Component({
  selector: 'app-promotion-table',
  imports: [SlicePipe, MatIconModule],
  templateUrl: './promotion-table.component.html',
})
export class PromotionTableComponent {
  readonly promotions = input.required<PromotionRow[]>();
  readonly loading = input(false);

  readonly edit = output<PromotionRow>();
  readonly delete = output<PromotionRow>();
  readonly applyProducts = output<PromotionRow>();

  protected readonly skeletonRows = Array.from({ length: 6 });
}
