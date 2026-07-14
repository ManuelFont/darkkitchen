export const Roles = {
  Customer: 'Customer',
  Dispatcher: 'Dispatcher',
  Administrator: 'Administrator',
} as const;

export type Role = (typeof Roles)[keyof typeof Roles];
export interface RoleRouteData {
  readonly roles: readonly Role[];
}
