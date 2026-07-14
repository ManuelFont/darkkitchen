import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of, startWith, switchMap } from 'rxjs';

import { AuditLogsFiltersComponent } from './audit-logs-filters/audit-logs-filters.component';
import { AuditLogsTableComponent } from './audit-logs-table/audit-logs-table.component';
import { MetricCardComponent } from './metric-card/metric-card.component';
import { MetricCardSkeletonComponent } from './metric-card-skeleton/metric-card-skeleton.component';
import { AuditLog } from './models/audit-log.model';
import { AuditLogFilters } from './models/audit-log-filters.model';
import { AuditLogRow } from './models/audit-log-row.model';
import { AuditMetric } from './models/audit-metric.model';
import { AuditLogsService } from './services/audit-logs.service';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';

const ACTION_TEXT_CLASSES: Record<string, string> = {
  Created: 'text-success-400',
  Updated: 'text-brand-300',
  Deleted: 'text-accent-400',
};

const DEFAULT_ACTION_TEXT_CLASS = 'text-stone-300';

const ENTITY_ICONS: Record<string, string> = {
  Product: 'restaurant',
  Promotion: 'sell',
};

const NO_FILTERS: AuditLogFilters = { entityName: '', userEmail: '', action: '' };

interface LogsState {
  readonly loading: boolean;
  readonly logs: AuditLog[];
}

const INITIAL_LOGS_STATE: LogsState = { loading: true, logs: [] };

@Component({
  selector: 'app-audit-logs',
  imports: [
    SidebarComponent,
    MetricCardSkeletonComponent,
    MetricCardComponent,
    AuditLogsFiltersComponent,
    AuditLogsTableComponent,
  ],
  templateUrl: './audit-logs.component.html',
})
export class AuditLogsComponent {
  private readonly auditLogsService = inject(AuditLogsService);

  private readonly filters = signal<AuditLogFilters>(NO_FILTERS);

  private readonly summary = toSignal(
    this.auditLogsService.getSummary().pipe(catchError(() => of(null))),
    { initialValue: null },
  );

  private readonly logsState = toSignal(
    toObservable(this.filters).pipe(
      switchMap((filters) =>
        this.auditLogsService.getAuditLogList(filters).pipe(
          map((logs) => ({ loading: false, logs }) satisfies LogsState),
          catchError(() => of<LogsState>({ loading: false, logs: [] })),
          startWith<LogsState>({ loading: true, logs: [] }),
        ),
      ),
    ),
    { initialValue: INITIAL_LOGS_STATE },
  );

  protected readonly isMetricsLoading = computed(() => this.summary() === null);
  protected readonly isLogsLoading = computed(() => this.logsState().loading);

  protected readonly metrics = computed<AuditMetric[]>(() => {
    const summary = this.summary();

    return [
      { label: 'Total events', icon: 'format_list_bulleted', value: summary?.total ?? 0 },
      { label: 'Creations', icon: 'add', value: summary?.created ?? 0 },
      { label: 'Updates', icon: 'edit', value: summary?.updated ?? 0 },
      { label: 'Deletions', icon: 'delete', value: summary?.deleted ?? 0 },
    ];
  });

  protected readonly logs = computed<AuditLogRow[]>(() =>
    this.logsState().logs.map((log) => {
      return {
        id: log.id,
        timestamp: log.timestamp,
        action: log.action,
        actionClass: ACTION_TEXT_CLASSES[log.action] ?? DEFAULT_ACTION_TEXT_CLASS,
        entityName: log.entityName,
        entityIcon: ENTITY_ICONS[log.entityName] ?? 'label',
        userEmail: log.userEmail,
        description: log.description,
      };
    }),
  );

  protected applyFilters(filters: AuditLogFilters): void {
    this.filters.set(filters);
  }
}
