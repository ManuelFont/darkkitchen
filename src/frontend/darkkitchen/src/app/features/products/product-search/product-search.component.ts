import { Component, output, signal } from '@angular/core';

@Component({
  selector: 'app-product-search',
  imports: [],
  templateUrl: './product-search.component.html',
  styleUrl: './product-search.component.css',
})
export class ProductSearchComponent {
  readonly searchText = signal('');
  readonly searchRequested = output<string>();

  updateSearchText(event: Event): void {
    this.searchText.set((event.target as HTMLInputElement).value);
  }

  search(): void {
    this.searchRequested.emit(this.searchText());
    this.searchText.set('');
  }
}
