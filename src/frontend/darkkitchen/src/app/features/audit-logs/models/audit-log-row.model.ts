export interface AuditLogRow {
  readonly id: string;
  readonly timestamp: string;
  readonly action: string;
  readonly actionClass: string;
  readonly entityName: string;
  readonly entityIcon: string;
  readonly userEmail: string;
  readonly description: string;
}
