import { Link } from 'react-router-dom'
import type { ReactNode } from 'react'
import { useLocale } from '../context/LocaleContext'

export type TableExtraAction = {
  label: string
  onClick?: () => void
  href?: string
  variant?: 'default' | 'accent' | 'danger'
  disabled?: boolean
}

function TableActionButton({ action }: { action: TableExtraAction }) {
  const className = `ias-table-action-btn ias-table-action-btn--${action.variant ?? 'default'}`

  if (action.href) {
    return (
      <Link to={action.href} className={className}>
        {action.label}
      </Link>
    )
  }

  return (
    <button
      type="button"
      onClick={action.onClick}
      disabled={action.disabled}
      className={className}
    >
      {action.label}
    </button>
  )
}

export function TableActions({
  onEdit,
  onDelete,
  deleteDisabled,
  leading,
  extra,
}: {
  onEdit: () => void
  onDelete: () => void
  deleteDisabled?: boolean
  leading?: TableExtraAction[]
  extra?: TableExtraAction[]
}) {
  const { t } = useLocale()

  return (
    <div className="ias-table-actions">
      {leading?.map((action) => (
        <TableActionButton key={action.label} action={action} />
      ))}
      <button type="button" onClick={onEdit} className="ias-table-action-btn ias-table-action-btn--default">
        {t('common.edit')}
      </button>
      {extra?.map((action) => (
        <TableActionButton key={action.label} action={action} />
      ))}
      <button
        type="button"
        onClick={onDelete}
        disabled={deleteDisabled}
        className="ias-table-action-btn ias-table-action-btn--danger"
      >
        {t('common.delete')}
      </button>
    </div>
  )
}

export function TableActionsSlot({ children }: { children: ReactNode }) {
  return <div className="ias-table-actions">{children}</div>
}
