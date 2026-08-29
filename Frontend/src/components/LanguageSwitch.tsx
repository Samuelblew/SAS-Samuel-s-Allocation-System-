import type { Locale } from '../i18n/messages'
import { useLocale } from '../context/LocaleContext'

export function LanguageSwitch({
  locale,
  onChange,
}: {
  locale: Locale
  onChange: (locale: Locale) => void
}) {
  const { t } = useLocale()

  return (
    <div className="ias-segment" role="group" aria-label={t('layout.language')}>
      {(['pt', 'en'] as const).map((code) => (
        <button
          key={code}
          type="button"
          onClick={() => onChange(code)}
          className={`ias-segment-btn ${locale === code ? 'ias-segment-btn-active' : ''}`}
        >
          {code.toUpperCase()}
        </button>
      ))}
    </div>
  )
}
