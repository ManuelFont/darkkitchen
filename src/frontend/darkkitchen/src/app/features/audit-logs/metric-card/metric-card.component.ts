import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { AuditMetric } from '../models/audit-metric.model';

@Component({
  selector: 'app-metric-card',
  imports: [MatIconModule],
  templateUrl: './metric-card.component.html',
})
export class MetricCardComponent {
  readonly metric = input.required<AuditMetric>();
}
