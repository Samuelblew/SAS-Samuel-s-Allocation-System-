import type { ApiErrorBody } from './types'

const baseUrl = (import.meta.env.VITE_API_URL as string | undefined)?.replace(/\/$/, '') ?? ''

export class ApiError extends Error {
  status: number
  body: ApiErrorBody | null

  constructor(status: number, message: string, body: ApiErrorBody | null = null) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.body = body
  }
}

export interface RequestOptions {
  tenantId: string
  actorId?: string
  signal?: AbortSignal
}

async function parseError(response: Response): Promise<ApiError> {
  let body: ApiErrorBody | null = null
  try {
    body = (await response.json()) as ApiErrorBody
  } catch {
    body = null
  }
  const message = body?.detail ?? body?.title ?? response.statusText ?? 'Erro na API'
  return new ApiError(response.status, message, body)
}

export async function apiFetch<T>(
  path: string,
  options: RequestOptions & { method?: string; body?: unknown } = { tenantId: '' },
): Promise<T> {
  const headers: Record<string, string> = {
    Accept: 'application/json',
  }

  if (options.tenantId) {
    headers['X-Tenant-Id'] = options.tenantId
  }
  if (options.actorId) {
    headers['X-Actor-Id'] = options.actorId
  }

  const init: RequestInit = {
    method: options.method ?? 'GET',
    headers,
    signal: options.signal,
  }

  if (options.body !== undefined) {
    headers['Content-Type'] = 'application/json'
    init.body = JSON.stringify(options.body)
  }

  const response = await fetch(`${baseUrl}${path}`, init)
  if (!response.ok) {
    throw await parseError(response)
  }
  if (response.status === 204) {
    return undefined as T
  }
  return (await response.json()) as T
}

export const api = {
  get: <T>(path: string, opts: RequestOptions) => apiFetch<T>(path, opts),
  post: <T>(path: string, body: unknown, opts: RequestOptions) =>
    apiFetch<T>(path, { ...opts, method: 'POST', body }),
  put: <T>(path: string, body: unknown, opts: RequestOptions) =>
    apiFetch<T>(path, { ...opts, method: 'PUT', body }),
  delete: (path: string, opts: RequestOptions) =>
    apiFetch<void>(path, { ...opts, method: 'DELETE' }),
}
