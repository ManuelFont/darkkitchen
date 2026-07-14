import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of, startWith, switchMap } from 'rxjs';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { ToastService } from '../../../core/toast/toast.service';
import { DateRangeFilterComponent } from '../date-range-filter/date-range-filter.component';
import { TopProductsChartComponent } from './top-products-chart/top-products-chart.component';
import { DateRange } from '../models/date-range.model';
import { TopProduct } from '../models/top-product.model';
import { TopProductBar } from '../models/top-product-bar.model';
import { ReportsService } from '../services/reports.service';

const NO_RANGE: DateRange = { dateFrom: '', dateTo: '' };

interface TopProductsState {
  readonly loading: boolean;
  readonly products: TopProduct[];
}

const INITIAL_STATE: TopProductsState = { loading: true, products: [] };

@Component({
  selector: 'app-top-products',
  imports: [SidebarComponent, DateRangeFilterComponent, TopProductsChartComponent],
  templateUrl: './top-products.component.html',
})
export class TopProductsComponent {
  private readonly reportsService = inject(ReportsService);
  private readonly toast = inject(ToastService);

  private readonly range = signal<DateRange>(NO_RANGE);

  private readonly state = toSignal(
    toObservable(this.range).pipe(
      switchMap((range) =>
        this.reportsService.getTopProducts(range).pipe(
          map((products) => ({ loading: false, products }) satisfies TopProductsState),
          catchError((error: Error) => {
            this.toast.error(error.message);
            return of<TopProductsState>({ loading: false, products: [] });
          }),
          startWith<TopProductsState>({ loading: true, products: [] }),
        ),
      ),
    ),
    { initialValue: INITIAL_STATE },
  );

  protected readonly isLoading = computed(() => this.state().loading);

  protected readonly bars = computed<TopProductBar[]>(() => {
    const products = [...this.state().products].sort((a, b) => b.quantity - a.quantity);
    const maxQuantity = products.reduce((max, product) => Math.max(max, product.quantity), 0);

    return products.map((product) => ({
      name: product.name,
      quantity: product.quantity,
      widthPct: maxQuantity > 0 ? (product.quantity / maxQuantity) * 100 : 0,
    }));
  });

  protected applyRange(range: DateRange): void {
    this.range.set(range);
  }
}
