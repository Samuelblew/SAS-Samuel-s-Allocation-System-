import { useLocale } from '../context/LocaleContext'
import { inputClass, labelClass } from '../lib/ui'

interface PeriodFilterProps {
  from: string
  to: string
  onFromChange: (value: string) => void
  onToChange: (value: string) => void
}

export function PeriodFilter({ from, to, onFromChange, onToChange }: PeriodFilterProps) {
  const { t } = useLocale()

  return (
    <div className="ias-period-filter">
      <label className={labelClass}>
        {t('common.from')}
        <input type="date" value={from} onChange={(e) => onFromChange(e.target.value)} className={inputClass} />
      </label>
      <label className={labelClass}>
        {t('common.to')}
        <input type="date" value={to} onChange={(e) => onToChange(e.target.value)} className={inputClass} />
      </label>
    </div>
  )
}
