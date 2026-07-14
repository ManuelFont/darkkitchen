import { Component, input } from '@angular/core';
import { TopProductBar } from '../../models/top-product-bar.model';

@Component({
  selector: 'app-top-products-chart',
  templateUrl: './top-products-chart.component.html',
})
export class TopProductsChartComponent {
  readonly bars = input.required<TopProductBar[]>();
  readonly loading = input(false);

  protected readonly skeletonRows = Array.from({ length: 5 });
}
