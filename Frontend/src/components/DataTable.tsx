import type { ReactNode } from 'react'

export function DataTable({ children }: { children: ReactNode }) {
  return (
    <div className="ias-table-wrap">
      <table className="ias-table">{children}</table>
    </div>
  )
}

export function DataTableHead({ children }: { children: ReactNode }) {
  return <thead className="ias-table-head">{children}</thead>
}

export function DataTableBody({ children }: { children: ReactNode }) {
  return <tbody className="ias-table-body">{children}</tbody>
}

export function DataTableEmpty({ colSpan, message }: { colSpan: number; message: string }) {
  return (
    <tr>
      <td colSpan={colSpan} className="ias-table-empty">
        {message}
      </td>
    </tr>
  )
}
