import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { ProductImportResult } from '../models/product-import-result.model';
import { ProductImporter } from '../models/product-importer.model';
import { ProductImportsService } from '../services/product-imports.service';

@Component({
  selector: 'app-product-import-modal',
  imports: [FormsModule],
  templateUrl: './product-import-modal.component.html',
})
export class ProductImportModalComponent {
  private readonly productImportsService = inject(ProductImportsService);

  readonly closeRequested = output<void>();
  readonly importCompleted = output<ProductImportResult>();
  readonly importers = signal<ProductImporter[]>([]);
  readonly isLoadingImporters = signal(true);
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly result = signal<ProductImportResult | null>(null);
  readonly hasAttemptedSubmit = signal(false);

  selectedImporterId = '';
  selectedFile: File | null = null;

  constructor() {
    this.loadImporters();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
    this.result.set(null);
    this.errorMessage.set(null);
  }

  onSubmit(): void {
    if (this.isSubmitting()) {
      return;
    }

    this.hasAttemptedSubmit.set(true);
    this.errorMessage.set(null);
    this.result.set(null);

    if (!this.selectedImporterId || !this.selectedFile) {
      this.errorMessage.set('Select an importer and a file');
      return;
    }

    if (this.selectedFile.size === 0) {
      this.errorMessage.set('File cannot be empty');
      return;
    }

    this.importProducts(this.selectedImporterId, this.selectedFile);
  }

  private loadImporters(): void {
    this.productImportsService
      .getImporters()
      .pipe(finalize(() => this.isLoadingImporters.set(false)))
      .subscribe({
        next: (importers: ProductImporter[]) => {
          this.importers.set(importers);
          this.selectedImporterId = importers[0]?.id ?? '';

          if (importers.length === 0) {
            this.errorMessage.set('No product importers are available');
          }
        },
        error: (error: Error) => this.errorMessage.set(error.message),
      });
  }

  private importProducts(importerId: string, file: File): void {
    this.isSubmitting.set(true);

    this.productImportsService
      .importProducts(importerId, file)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (result: ProductImportResult) => {
          this.result.set(result);
          this.importCompleted.emit(result);
        },
        error: (error: Error) => this.errorMessage.set(error.message),
      });
  }
}
