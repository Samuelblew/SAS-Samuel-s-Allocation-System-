import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { Alert } from '../components/Alert'
import { DataTable, DataTableBody, DataTableHead } from '../components/DataTable'
import { EmptyState } from '../components/EmptyState'
import { FormField } from '../components/forms/FormField'
import { LoadingState } from '../components/LoadingState'
import { PageHeader } from '../components/PageHeader'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { PageSection } from '../components/page/PageSection'
import { TableActions } from '../components/TableActions'
import {
  TableCellChip,
  TableCellIdentity,
} from '../components/table/TableCells'
import { useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { api } from '../lib/api'
import { getErrorMessage } from '../lib/errors'
import { skillFormSchema, toSkillPayload, type SkillFormValues } from '../lib/schemas/skill'
import { btnGhostClass, btnPrimaryClass, formRowClass, inputClass } from '../lib/ui'
import type { Paged, Skill } from '../lib/types'

const defaultValues: SkillFormValues = { name: '', category: '' }

export function SkillsPage() {
  const { t } = useLocale()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const qc = useQueryClient()
  const [editingId, setEditingId] = useState<string | null>(null)
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm<SkillFormValues>({
    resolver: zodResolver(skillFormSchema),
    defaultValues,
  })

  const query = useQuery({
    queryKey: ['skills', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<Skill>>('/api/v1/skills?page=1&pageSize=100', opts),
  })

  function resetForm() {
    form.reset(defaultValues)
    setEditingId(null)
    setFormError(null)
  }

  function startEdit(skill: Skill) {
    setEditingId(skill.id)
    form.reset({ name: skill.name, category: skill.category ?? '' })
    setFormError(null)
  }

  const create = useMutation({
    mutationFn: (body: ReturnType<typeof toSkillPayload>) =>
      api.post<Skill>('/api/v1/skills', body, opts),
    onSuccess: () => {
      resetForm()
      void qc.invalidateQueries({ queryKey: ['skills'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const update = useMutation({
    mutationFn: ({ id, body }: { id: string; body: ReturnType<typeof toSkillPayload> }) =>
      api.put<Skill>(`/api/v1/skills/${id}`, body, opts),
    onSuccess: () => {
      resetForm()
      void qc.invalidateQueries({ queryKey: ['skills'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const remove = useMutation({
    mutationFn: (id: string) => api.delete(`/api/v1/skills/${id}`, opts),
    onSuccess: () => {
      if (editingId) resetForm()
      void qc.invalidateQueries({ queryKey: ['skills'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const onSubmit = form.handleSubmit((values) => {
    const body = toSkillPayload(values)
    if (editingId) {
      update.mutate({ id: editingId, body })
    } else {
      create.mutate(body)
    }
  })

  const categoryCount =
    query.data?.items.filter((skill) => skill.category && skill.category.trim()).length ?? 0

  return (
    <div>
      <PageHeader title={t('pages.skills.title')} description={t('pages.skills.description')} hideTitle />

      <PageGuide
        title={t('guide.howToRead')}
        steps={[
          t('pages.skills.guide.step1'),
          t('pages.skills.guide.step2'),
          t('pages.skills.guide.step3'),
        ]}
      />

      {query.isSuccess && (
        <PageHero
          label={t('pages.skills.hero.label')}
          value={query.data.totalCount}
          hint={t('pages.skills.hero.hint')}
          metrics={
            <PageHeroMetric label={t('pages.skills.hero.categories')} value={categoryCount} />
          }
        />
      )}

      <PageSection
        title={editingId ? t('form.skills.edit') : t('form.skills.new')}
        className="mb-6"
      >
        <form onSubmit={onSubmit} className={formRowClass}>
          <FormField label={t('common.name')} error={form.formState.errors.name?.message} className="min-w-[200px]">
            <input {...form.register('name')} className={inputClass} />
          </FormField>
          <FormField label={t('common.category')} className="min-w-[160px]">
            <input {...form.register('category')} className={inputClass} />
          </FormField>
          <button
            type="submit"
            disabled={!isTenantValid || create.isPending || update.isPending}
            className={btnPrimaryClass}
          >
            {editingId ? t('common.save') : t('common.add')}
          </button>
          {editingId && (
            <button type="button" onClick={resetForm} className={btnGhostClass}>
              {t('common.cancel')}
            </button>
          )}
        </form>
        {formError && (
          <div className="mt-3">
            <Alert message={formError} />
          </div>
        )}
      </PageSection>

      {query.isLoading && <LoadingState />}
      {query.isError && <Alert message={getErrorMessage(query.error)} />}
      {query.isSuccess && query.data.items.length === 0 && <EmptyState message={t('empty.skills')} />}
      {query.isSuccess && query.data.items.length > 0 && (
        <DataTable>
          <DataTableHead>
            <tr>
              <th>{t('common.name')}</th>
              <th>{t('common.category')}</th>
              <th>{t('common.actions')}</th>
            </tr>
          </DataTableHead>
          <DataTableBody>
            {query.data.items.map((s) => (
              <tr key={s.id}>
                <td>
                  <TableCellIdentity title={s.name} />
                </td>
                <td>
                  <TableCellChip label={s.category} />
                </td>
                <td>
                  <TableActions
                    onEdit={() => startEdit(s)}
                    onDelete={() => {
                      if (window.confirm(t('common.confirmDelete'))) remove.mutate(s.id)
                    }}
                    deleteDisabled={remove.isPending}
                  />
                </td>
              </tr>
            ))}
          </DataTableBody>
        </DataTable>
      )}
    </div>
  )
}
