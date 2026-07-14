export interface OrderStatusOption {
  readonly value: number;
  readonly name: string;
}

export const ORDER_STATUS_OPTIONS: readonly OrderStatusOption[] = [
  { value: 0, name: 'Pending' },
  { value: 1, name: 'Ready' },
  { value: 2, name: 'Cancelled' },
  { value: 3, name: 'OnTheWay' },
  { value: 4, name: 'Delivered' },
  { value: 5, name: 'NotDelivered' },
  { value: 6, name: 'Delayed' },
];

export interface StatusBadge {
  readonly label: string;
  readonly badgeClass: string;
  readonly dotClass: string;
}

export const STATUS_BADGES: Record<string, StatusBadge> = {
  Pending: {
    label: 'Pending',
    badgeClass: 'bg-info-500/15 text-info-300',
    dotClass: 'bg-info-400',
  },
  Ready: { label: 'Ready', badgeClass: 'bg-brand-500/15 text-brand-300', dotClass: 'bg-brand-400' },
  OnTheWay: {
    label: 'On the way',
    badgeClass: 'bg-brand-500/15 text-brand-300',
    dotClass: 'bg-brand-400',
  },
  Delivered: {
    label: 'Delivered',
    badgeClass: 'bg-success-500/15 text-success-300',
    dotClass: 'bg-success-400',
  },
  Cancelled: {
    label: 'Cancelled',
    badgeClass: 'bg-accent-500/15 text-accent-300',
    dotClass: 'bg-accent-400',
  },
  NotDelivered: {
    label: 'Not delivered',
    badgeClass: 'bg-accent-500/15 text-accent-300',
    dotClass: 'bg-accent-400',
  },
  Delayed: {
    label: 'Delayed',
    badgeClass: 'bg-accent-500/15 text-accent-300',
    dotClass: 'bg-accent-400',
  },
};

export const DEFAULT_STATUS_BADGE: StatusBadge = {
  label: 'Unknown',
  badgeClass: 'bg-stone-500/15 text-stone-300',
  dotClass: 'bg-stone-400',
};
