import { Component, computed, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { ToastVariant } from './toast.model';
import { ToastService } from './toast.service';

interface ToastView {
  readonly id: number;
  readonly message: string;
  readonly containerClass: string;
  readonly iconClass: string;
  readonly icon: string;
}

interface ToastVariantStyle {
  readonly containerClass: string;
  readonly iconClass: string;
  readonly icon: string;
}

const VARIANT_STYLES: Record<ToastVariant, ToastVariantStyle> = {
  success: {
    containerClass: 'border-success-700 bg-success-900 text-success-50',
    iconClass: 'text-success-300',
    icon: 'check_circle',
  },
  error: {
    containerClass: 'border-accent-700 bg-accent-900 text-accent-50',
    iconClass: 'text-accent-300',
    icon: 'error',
  },
  info: {
    containerClass: 'border-info-700 bg-info-900 text-info-50',
    iconClass: 'text-info-300',
    icon: 'info',
  },
};

@Component({
  selector: 'app-toast-container',
  imports: [MatIconModule],
  templateUrl: './toast-container.component.html',
})
export class ToastContainerComponent {
  private readonly toastService = inject(ToastService);

  protected readonly toasts = computed<ToastView[]>(() =>
    this.toastService.toasts().map((toast) => ({
      id: toast.id,
      message: toast.message,
      ...VARIANT_STYLES[toast.variant],
    })),
  );

  protected dismiss(id: number): void {
    this.toastService.dismiss(id);
  }
}
