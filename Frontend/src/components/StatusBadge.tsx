const statusClass: Record<string, string> = {
  Active: 'ias-status-badge--Active',
  Contractor: 'ias-status-badge--Active',
  Confirmed: 'ias-status-badge--Confirmed',
  InProgress: 'ias-status-badge--InProgress',
  Planned: 'ias-status-badge--Planned',
  AtRisk: 'ias-status-badge--AtRisk',
  Vacation: 'ias-status-badge--Vacation',
  NoticePeriod: 'ias-status-badge--Vacation',
  Offboarded: 'ias-status-badge--Offboarded',
  Closed: 'ias-status-badge--Closed',
  Open: 'ias-status-badge--Open',
  PartiallyFilled: 'ias-status-badge--PartiallyFilled',
  Filled: 'ias-status-badge--Filled',
  Proposal: 'ias-status-badge--Planned',
  Approved: 'ias-status-badge--Confirmed',
  Paused: 'ias-status-badge--AtRisk',
}

export function StatusBadge({ label, status }: { label: string; status?: string }) {
  const tone = status ? (statusClass[status] ?? 'ias-status-badge--default') : 'ias-status-badge--default'

  return <span className={`ias-status-badge ${tone}`}>{label}</span>
}
