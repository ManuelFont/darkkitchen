export interface UserRoleOption {
  readonly id: number;
  readonly name: string;
}

export const USER_ROLES: readonly UserRoleOption[] = [
  { id: 1, name: 'Customer' },
  { id: 2, name: 'Dispatcher' },
  { id: 3, name: 'Administrator' },
];
