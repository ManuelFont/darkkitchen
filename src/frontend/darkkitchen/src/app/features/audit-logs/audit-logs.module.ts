import { NgModule } from '@angular/core';
import { AuditLogsComponent } from './audit-logs.component';
import { AuditLogsRoutingModule } from './audit-logs-routing.module';

@NgModule({
  imports: [AuditLogsComponent, AuditLogsRoutingModule],
})
export class AuditLogsModule {}
