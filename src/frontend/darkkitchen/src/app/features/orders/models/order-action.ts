import { Role, Roles } from '../../../core/auth/role.model';

export type OrderAction = 'ready' | 'cancel' | 'delayed' | 'onTheWay' | 'deliver' | 'notDelivered';

export interface OrderActionDescriptor {
  readonly action: OrderAction;
  readonly label: string;
  readonly endpoint: string;
  readonly fromStatuses: readonly string[];
  readonly roles: readonly Role[];
  readonly variant: 'primary' | 'danger';
}

export const ORDER_ACTIONS: readonly OrderActionDescriptor[] = [
  {
    action: 'ready',
    label: 'Mark ready',
    endpoint: '/ready',
    fromStatuses: ['Pending', 'Delayed'],
    roles: [Roles.Dispatcher, Roles.Administrator],
    variant: 'primary',
  },
  {
    action: 'cancel',
    label: 'Cancel',
    endpoint: '/cancel',
    fromStatuses: ['Pending', 'Delayed'],
    roles: [Roles.Administrator],
    variant: 'danger',
  },
  {
    action: 'delayed',
    label: 'Mark delayed',
    endpoint: '/delayed',
    fromStatuses: ['Pending'],
    roles: [Roles.Dispatcher],
    variant: 'primary',
  },
  {
    action: 'onTheWay',
    label: 'Mark on the way',
    endpoint: '/on-the-way',
    fromStatuses: ['Ready'],
    roles: [Roles.Dispatcher],
    variant: 'primary',
  },
  {
    action: 'deliver',
    label: 'Mark delivered',
    endpoint: '/deliver',
    fromStatuses: ['OnTheWay'],
    roles: [Roles.Dispatcher],
    variant: 'primary',
  },
  {
    action: 'notDelivered',
    label: 'Mark not delivered',
    endpoint: '/not-delivered',
    fromStatuses: ['OnTheWay'],
    roles: [Roles.Dispatcher],
    variant: 'danger',
  },
];

export function availableActions(status: string, role: Role | null): OrderActionDescriptor[] {
  if (role === null) {
    return [];
  }

  return ORDER_ACTIONS.filter(
    (descriptor) => descriptor.fromStatuses.includes(status) && descriptor.roles.includes(role),
  );
}
