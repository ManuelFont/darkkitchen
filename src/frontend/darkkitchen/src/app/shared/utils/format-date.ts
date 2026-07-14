const MONTEVIDEO_TIME_ZONE = 'America/Montevideo';
const HAS_TIME_ZONE = /([zZ]|[+-]\d{2}:?\d{2})$/;

export function formatMontevideoDateTime(value: string): string {
  const normalized = HAS_TIME_ZONE.test(value) ? value : `${value}Z`;
  const date = new Date(normalized);

  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return date.toLocaleString(undefined, {
    timeZone: MONTEVIDEO_TIME_ZONE,
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}
