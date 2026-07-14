export interface AuditLog {
  id: string;
  timestamp: string;
  action: string;
  entityName: string;
  description: string;
  userEmail: string;
}
