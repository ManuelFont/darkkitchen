import { Component, input, output } from '@angular/core';
import { PromotionRow } from '../models/promotion-row.model';

@Component({
  selector: 'app-promotion-delete-modal',
  imports: [],
  templateUrl: './promotion-delete-modal.component.html',
})
export class PromotionDeleteModalComponent {
  readonly promotion = input.required<PromotionRow>();
  readonly isDeleting = input(false);

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();
}
