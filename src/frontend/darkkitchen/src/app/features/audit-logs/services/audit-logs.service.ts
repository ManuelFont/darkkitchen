import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuditLog } from '../models/audit-log.model';
import { AuditLogFilters } from '../models/audit-log-filters.model';
import { AuditSummary } from '../models/audit-summary.model';

@Injectable({
  providedIn: 'root',
})
export class AuditLogsService {
  private readonly http = inject(HttpClient);
  private readonly auditLogsUrl = `${environment.apiBaseUrl}/audit-logs`;

  public getSummary(): Observable<AuditSummary> {
    return this.http.get<AuditSummary>(`${this.auditLogsUrl}/summary`);
  }

  public getAuditLogList(filters: AuditLogFilters): Observable<AuditLog[]> {
    return this.http.get<AuditLog[]>(this.auditLogsUrl, { params: this.buildParams(filters) });
  }

  private buildParams({ entityName, userEmail, action }: AuditLogFilters): HttpParams {
    let params = new HttpParams();

    if (entityName) {
      params = params.set('entityName', entityName);
    }

    if (userEmail) {
      params = params.set('userEmail', userEmail);
    }

    if (action) {
      params = params.set('action', action);
    }

    return params;
  }
}
