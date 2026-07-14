import { ProductImportIssue } from './product-import-issue.model';

export interface ProductImportResult {
  importerId: string;
  total: number;
  created: number;
  skippedDuplicates: number;
  failed: number;
  issues: ProductImportIssue[];
}
