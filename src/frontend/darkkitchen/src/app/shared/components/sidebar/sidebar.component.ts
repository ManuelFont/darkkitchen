import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { Roles } from '../../../core/auth/role.model';
import { NavItemComponent } from './nav-item/nav-item.component';

type NavItemAction = 'logout';

interface NavItem {
  label: string;
  icon: string;
  route?: string;
  action?: NavItemAction;
}

interface NavSection {
  title: string;
  items: NavItem[];
}

@Component({
  selector: 'app-sidebar',
  imports: [NavItemComponent],
  templateUrl: './sidebar.component.html',
  host: {
    class: 'flex h-dvh w-64 flex-col bg-surface text-stone-300 select-none',
  },
})
export class SidebarComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly activeLabel = signal(this.getActiveLabel());

  private readonly customerLabels = new Set(['Orders', 'Products', 'Logout']);
  private readonly dispatcherLabels = new Set(['Orders', 'Logout']);
  private readonly adminLabels = new Set([
    'Dashboard',
    'Orders',
    'Products',
    'Promotions',
    'Delivery Types',
    'Top Products',
    'Sales Report',
    'Users',
    'Audit Logs',
    'Logout',
  ]);

  private readonly allSections: NavSection[] = [
    {
      title: 'General',
      items: [
        { label: 'Dashboard', icon: 'grid_view', route: '/home' },
        { label: 'Orders', icon: 'assignment', route: '/orders' },
      ],
    },
    {
      title: 'Catalog',
      items: [
        { label: 'Products', icon: 'restaurant', route: '/products' },
        { label: 'Promotions', icon: 'sell', route: '/promotions' },
        { label: 'Delivery Types', icon: 'local_shipping', route: '/delivery-types' },
      ],
    },
    {
      title: 'Reports',
      items: [
        { label: 'Top Products', icon: 'bar_chart', route: '/reports/top-products' },
        { label: 'Sales Report', icon: 'receipt_long', route: '/reports/sales' },
      ],
    },
    {
      title: 'System',
      items: [
        { label: 'Users', icon: 'group', route: '/users' },
        { label: 'Audit Logs', icon: 'history', route: '/audit-logs' },
        { label: 'Logout', icon: 'logout', action: 'logout' },
      ],
    },
  ];

  readonly sections: NavSection[] = this.buildSections();

  private buildSections(): NavSection[] {
    const allowedLabels = this.allowedLabelsForRole();

    return this.allSections
      .map((section) => ({
        ...section,
        items: section.items.filter((item) => allowedLabels.has(item.label)),
      }))
      .filter((section) => section.items.length > 0);
  }

  private allowedLabelsForRole(): Set<string> {
    if (this.authService.hasRole(Roles.Administrator)) {
      return this.adminLabels;
    }

    if (this.authService.hasRole(Roles.Dispatcher)) {
      return this.dispatcherLabels;
    }

    return this.customerLabels;
  }

  select(item: NavItem): void {
    if (item.action === 'logout') {
      this.authService.logout();
      void this.router.navigateByUrl('/login');
    } else {
      this.activeLabel.set(item.label);
      if (item.route) {
        void this.router.navigateByUrl(item.route);
      }
    }
  }

  private getActiveLabel(): string {
    if (this.router.url.startsWith('/reports/top-products')) {
      return 'Top Products';
    }

    if (this.router.url.startsWith('/reports/sales')) {
      return 'Sales Report';
    }

    if (this.router.url.startsWith('/audit-logs')) {
      return 'Audit Logs';
    }

    if (this.router.url.startsWith('/users')) {
      return 'Users';
    }

    if (this.router.url.startsWith('/products')) {
      return 'Products';
    }

    if (this.router.url.startsWith('/promotions')) {
      return 'Promotions';
    }

    if (this.router.url.startsWith('/delivery-types')) {
      return 'Delivery Types';
    }

    if (this.router.url.startsWith('/orders')) {
      return 'Orders';
    }

    return 'Dashboard';
  }
}
