import type { ReactElement } from 'react'
import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { LoadingState } from '../components/LoadingState'
import { LocaleProvider } from '../context/LocaleContext'

function renderWithLocale(ui: ReactElement) {
  return render(<LocaleProvider>{ui}</LocaleProvider>)
}

describe('LoadingState', () => {
  it('renderiza texto de carregamento em pt', () => {
    renderWithLocale(<LoadingState />)
    expect(screen.getByText('Carregando…')).toBeInTheDocument()
  })
})
