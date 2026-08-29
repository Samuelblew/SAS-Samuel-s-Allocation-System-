import { describe, expect, it } from 'vitest'
import { ApiError } from './api'
import { getErrorMessage } from './errors'

describe('getErrorMessage', () => {
  it('extrai mensagem de ApiError', () => {
    const err = new ApiError(409, 'Conflito de alocação')
    expect(getErrorMessage(err)).toBe('Conflito de alocação')
  })

  it('extrai mensagem de Error genérico', () => {
    expect(getErrorMessage(new Error('falha'))).toBe('falha')
  })

  it('usa fallback para valor desconhecido', () => {
    expect(getErrorMessage(null, 'padrão')).toBe('padrão')
  })
})
