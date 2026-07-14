import { Component, computed, input, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MonthlySalesView } from '../../models/sales-report-view.model';

interface MonthlySalesDisplay extends MonthlySalesView {
  readonly expanded: boolean;
}

@Component({
  selector: 'app-sales-report-table',
  imports: [MatIconModule],
  templateUrl: './sales-report-table.component.html',
})
export class SalesReportTableComponent {
  readonly months = input.required<MonthlySalesView[]>();
  readonly loading = input(false);

  private readonly collapsed = signal<ReadonlySet<string>>(new Set());

  protected readonly skeletonRows = Array.from({ length: 3 });

  protected readonly displayMonths = computed<MonthlySalesDisplay[]>(() => {
    const collapsed = this.collapsed();
    return this.months().map((month) => ({ ...month, expanded: !collapsed.has(month.period) }));
  });

  protected toggle(period: string): void {
    this.collapsed.update((current) => {
      const next = new Set(current);
      if (next.has(period)) {
        next.delete(period);
      } else {
        next.add(period);
      }
      return next;
    });
  }
}
