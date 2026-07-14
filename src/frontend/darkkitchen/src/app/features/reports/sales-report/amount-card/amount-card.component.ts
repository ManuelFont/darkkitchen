import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { AmountCard } from '../../models/sales-report-view.model';

@Component({
  selector: 'app-amount-card',
  imports: [MatIconModule],
  templateUrl: './amount-card.component.html',
})
export class AmountCardComponent {
  readonly card = input.required<AmountCard>();
  readonly loading = input(false);
}
