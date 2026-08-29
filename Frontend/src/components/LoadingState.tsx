import { useLocale } from '../context/LocaleContext'

export function LoadingState({ message }: { message?: string }) {
  const { t } = useLocale()
  return <p className="ias-text-subtle text-sm">{message ?? t('common.loading')}</p>
}
