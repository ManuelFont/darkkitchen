import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { catchError, combineLatest, finalize, of, startWith, Subject, switchMap } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { Roles } from '../../../core/auth/role.model';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { CategoryEditModalComponent } from '../../categories/category-edit-modal/category-edit-modal.component';
import { CategoryDropdownComponent } from '../category-dropdown/category-dropdown.component';
import { ProductImportResult } from '../models/product-import-result.model';
import type { ProductFilters } from '../models/product-filters.model';
import { ProductCardComponent } from '../product-card/product-card.component';
import { ProductCreateModalComponent } from '../product-create-modal/product-create-modal.component';
import { ProductImportModalComponent } from '../product-import-modal/product-import-modal.component';
import { ProductSearchComponent } from '../product-search/product-search.component';
import { ProductsService } from '../services/products.service';

@Component({
  selector: 'app-product-list',
  imports: [
    CategoryEditModalComponent,
    CategoryDropdownComponent,
    MatIconModule,
    ProductCardComponent,
    ProductCreateModalComponent,
    ProductImportModalComponent,
    ProductSearchComponent,
    SidebarComponent,
  ],
  templateUrl: './product-list.component.html',
})
export class ProductListComponent {
  private readonly authService = inject(AuthService);
  private readonly productsService = inject(ProductsService);
  private readonly router = inject(Router);
  readonly canManageCatalog = computed(() =>
    this.authService.currentRole() === Roles.Administrator,
  );
  readonly isCategoryEditModalOpen = signal(false);
  readonly isLoading = signal(false);
  readonly isProductFormOpen = signal(false);
  readonly isProductImportOpen = signal(false);
  readonly canImportProducts = this.authService.hasRole(Roles.Administrator);
  readonly selectedCategoryId = signal('');
  readonly categoryRefreshVersion = signal(0);
  readonly successMessage = signal<string | null>(
    (this.router.currentNavigation()?.extras.state?.['successMessage'] as string | undefined) ??
      null,
  );
  private readonly productSearchName = signal('');
  private readonly refreshProductsRequested = new Subject<void>();
  private readonly filters = computed<ProductFilters>(() => ({
    categoryId: this.selectedCategoryId(),
    name: this.productSearchName(),
  }));

  readonly products = toSignal(
    combineLatest([
      toObservable(this.filters),
      this.refreshProductsRequested.pipe(startWith(undefined)),
    ]).pipe(
      switchMap(([filters]) => {
        this.isLoading.set(true);

        return this.productsService.getAll(filters).pipe(
          catchError(() => of([])),
          finalize(() => this.isLoading.set(false)),
        );
      }),
    ),
    { initialValue: [] },
  );

  openProductForm(): void {
    if (!this.canManageCatalog()) {
      return;
    }

    this.isProductFormOpen.set(true);
  }

  closeProductForm(): void {
    this.isProductFormOpen.set(false);
  }

  openCategoryEditModal(): void {
    if (!this.canManageCatalog()) {
      return;
    }

    this.isCategoryEditModalOpen.set(true);
  }

  closeCategoryEditModal(): void {
    this.isCategoryEditModalOpen.set(false);
    this.categoryRefreshVersion.update((version) => version + 1);

    if (this.selectedCategoryId()) {
      this.selectedCategoryId.set('');
      return;
    }

    this.refreshProductsRequested.next();
  }

  openProductImport(): void {
    if (!this.canImportProducts) {
      return;
    }

    this.isProductImportOpen.set(true);
  }

  closeProductImport(): void {
    this.isProductImportOpen.set(false);
  }

  handleProductCreated(): void {
    this.closeProductForm();
    this.refreshProductsRequested.next();
  }

  handleProductsImported(result: ProductImportResult): void {
    this.successMessage.set(
      `Imported ${result.created} products. ${result.skippedDuplicates} duplicates skipped, ${result.failed} failed.`,
    );
    this.refreshProductsRequested.next();
  }

  clearSuccessMessage(): void {
    this.successMessage.set(null);
  }

  selectCategory(categoryId: string): void {
    this.productSearchName.set('');
    this.selectedCategoryId.set(categoryId);
  }

  searchProducts(name: string): void {
    this.selectedCategoryId.set('');
    this.productSearchName.set(name);
  }
}
