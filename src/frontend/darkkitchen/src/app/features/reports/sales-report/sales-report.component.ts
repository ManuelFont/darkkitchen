import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of, startWith, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { DateRangeFilterComponent } from '../date-range-filter/date-range-filter.component';
import { AmountCardComponent } from './amount-card/amount-card.component';
import { SalesReportTableComponent } from './sales-report-table/sales-report-table.component';
import { formatMoney } from '../../../shared/utils/format-money';
import { DateRange } from '../models/date-range.model';
import { SalesAmounts, SalesReport } from '../models/sales-report.model';
import { AmountCard, MonthlySalesView, SalesAmountsView } from '../models/sales-report-view.model';
import { ReportsService } from '../services/reports.service';

const NO_RANGE: DateRange = { dateFrom: '', dateTo: '' };
const EMPTY_AMOUNTS: SalesAmounts = { items: 0, delivery: 0, iva: 0, total: 0 };
const MONTH_NAMES = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December',
];

interface SalesReportState {
  readonly loading: boolean;
  readonly report: SalesReport | null;
}

const INITIAL_STATE: SalesReportState = { loading: true, report: null };

@Component({
  selector: 'app-sales-report',
  imports: [
    SidebarComponent,
    DateRangeFilterComponent,
    AmountCardComponent,
    SalesReportTableComponent,
  ],
  templateUrl: './sales-report.component.html',
})
export class SalesReportComponent {
  private readonly reportsService = inject(ReportsService);
  private readonly toast = inject(ToastService);

  private readonly range = signal<DateRange>(NO_RANGE);

  private readonly state = toSignal(
    toObservable(this.range).pipe(
      switchMap((range) =>
        this.reportsService.getSalesReport(range).pipe(
          map((report) => ({ loading: false, report }) satisfies SalesReportState),
          catchError((error: Error) => {
            this.toast.error(error.message);
            return of<SalesReportState>({ loading: false, report: null });
          }),
          startWith<SalesReportState>({ loading: true, report: null }),
        ),
      ),
    ),
    { initialValue: INITIAL_STATE },
  );

  protected readonly isLoading = computed(() => this.state().loading);

  protected readonly grandTotalCards = computed<AmountCard[]>(() => {
    const totals = this.state().report?.grandTotal ?? EMPTY_AMOUNTS;

    return [
      { label: 'Items', icon: 'shopping_bag', value: formatMoney(totals.items) },
      { label: 'Delivery', icon: 'local_shipping', value: formatMoney(totals.delivery) },
      { label: 'IVA', icon: 'percent', value: formatMoney(totals.iva) },
      { label: 'Total', icon: 'payments', value: formatMoney(totals.total) },
    ];
  });

  protected readonly months = computed<MonthlySalesView[]>(() =>
    (this.state().report?.months ?? []).map((month) => ({
      period: month.period,
      label: `${MONTH_NAMES[month.month - 1]} ${month.year}`,
      clients: month.clients.map((client) => ({
        clientId: client.clientId,
        clientName: client.clientName,
        amounts: this.toAmountsView(client.amounts),
      })),
      subtotal: this.toAmountsView(month.subtotal),
    })),
  );

  protected applyRange(range: DateRange): void {
    this.range.set(range);
  }

  private toAmountsView(amounts: SalesAmounts): SalesAmountsView {
    return {
      items: formatMoney(amounts.items),
      delivery: formatMoney(amounts.delivery),
      iva: formatMoney(amounts.iva),
      total: formatMoney(amounts.total),
    };
  }
}
