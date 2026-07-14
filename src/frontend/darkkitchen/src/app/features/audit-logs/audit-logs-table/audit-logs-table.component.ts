import { DatePipe, SlicePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { AuditLogRow } from '../models/audit-log-row.model';

@Component({
  selector: 'app-audit-logs-table',
  imports: [DatePipe, SlicePipe, MatIconModule],
  templateUrl: './audit-logs-table.component.html',
})
export class AuditLogsTableComponent {
  readonly logs = input.required<AuditLogRow[]>();
  readonly loading = input(false);

  protected readonly skeletonRows = Array.from({ length: 8 });
}
