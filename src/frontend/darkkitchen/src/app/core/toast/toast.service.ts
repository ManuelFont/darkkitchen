import { Injectable, signal } from '@angular/core';
import { Toast, ToastVariant } from './toast.model';

const TOAST_DURATION_MS = 4000;

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private readonly toastList = signal<Toast[]>([]);
  private nextId = 0;

  readonly toasts = this.toastList.asReadonly();

  success(message: string): void {
    this.show(message, 'success');
  }

  error(message: string): void {
    this.show(message, 'error');
  }

  info(message: string): void {
    this.show(message, 'info');
  }

  dismiss(id: number): void {
    this.toastList.update((toasts) => toasts.filter((toast) => toast.id !== id));
  }

  private show(message: string, variant: ToastVariant): void {
    const id = this.nextId++;
    this.toastList.update((toasts) => [...toasts, { id, message, variant }]);
    setTimeout(() => this.dismiss(id), TOAST_DURATION_MS);
  }
}
