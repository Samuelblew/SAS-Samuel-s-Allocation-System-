import { useId, useState, type ReactNode } from 'react'

function Chevron({ open }: { open: boolean }) {
  return (
    <svg
      className={`ias-form-drawer-chevron${open ? ' ias-form-drawer-chevron--open' : ''}`}
      viewBox="0 0 12 12"
      fill="none"
      aria-hidden
    >
      <path
        d="M3 5l3 3 3-3"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

export function FormDrawer({
  title,
  summary,
  preview,
  defaultOpen = false,
  children,
}: {
  title: string
  summary?: string
  preview?: ReactNode
  defaultOpen?: boolean
  children: ReactNode
}) {
  const [open, setOpen] = useState(defaultOpen)
  const panelId = useId()

  return (
    <div className={`ias-form-drawer${open ? ' ias-form-drawer--open' : ''}`}>
      <button
        type="button"
        className="ias-form-drawer-trigger"
        aria-expanded={open}
        aria-controls={panelId}
        onClick={() => setOpen((value) => !value)}
      >
        <div className="ias-form-drawer-trigger-main">
          <span className="ias-form-drawer-title">{title}</span>
          {summary ? <span className="ias-form-drawer-summary">{summary}</span> : null}
          {!open && preview ? <div className="ias-form-drawer-preview">{preview}</div> : null}
        </div>
        <Chevron open={open} />
      </button>
      <div id={panelId} className="ias-form-drawer-panel" hidden={!open}>
        <div className="ias-form-drawer-panel-scroll">{children}</div>
      </div>
    </div>
  )
}
