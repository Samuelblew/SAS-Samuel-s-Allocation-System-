import type { MessageKey } from '../i18n/messages'
import type { NavIconName } from '../components/NavIcons'

export interface NavItem {
  to: string
  key: MessageKey
  icon: NavIconName
  end?: boolean
}

export interface NavGroup {
  labelKey: MessageKey
  items: NavItem[]
}

export const navGroups: NavGroup[] = [
  {
    labelKey: 'nav.group.overview',
    items: [{ to: '/', key: 'nav.dashboard', icon: 'dashboard', end: true }],
  },
  {
    labelKey: 'nav.group.team',
    items: [
      { to: '/skills', key: 'nav.skills', icon: 'skills' },
      { to: '/people', key: 'nav.people', icon: 'people' },
      { to: '/projects', key: 'nav.projects', icon: 'projects' },
    ],
  },
  {
    labelKey: 'nav.group.allocation',
    items: [
      { to: '/allocation-needs', key: 'nav.needs', icon: 'needs' },
      { to: '/allocations', key: 'nav.allocations', icon: 'allocations' },
      { to: '/matching', key: 'nav.matching', icon: 'matching' },
      { to: '/conflicts', key: 'nav.conflicts', icon: 'conflicts' },
    ],
  },
  {
    labelKey: 'nav.group.planning',
    items: [
      { to: '/simulations', key: 'nav.simulations', icon: 'simulations' },
      { to: '/capacity', key: 'nav.capacity', icon: 'capacity' },
    ],
  },
  {
    labelKey: 'nav.group.finance',
    items: [{ to: '/financials', key: 'nav.financials', icon: 'financials' }],
  },
]

const routePageKeys: Record<string, MessageKey> = {
  '/': 'pages.dashboard.title',
  '/skills': 'pages.skills.title',
  '/people': 'pages.people.title',
  '/projects': 'pages.projects.title',
  '/allocation-needs': 'pages.needs.title',
  '/allocations': 'pages.allocations.title',
  '/simulations': 'pages.simulations.title',
  '/capacity': 'pages.capacity.title',
  '/matching': 'pages.matching.title',
  '/financials': 'pages.financials.title',
  '/conflicts': 'pages.conflicts.title',
}

const routeGroupKeys: Record<string, MessageKey> = {
  '/': 'nav.group.overview',
  '/skills': 'nav.group.team',
  '/people': 'nav.group.team',
  '/projects': 'nav.group.team',
  '/allocation-needs': 'nav.group.allocation',
  '/allocations': 'nav.group.allocation',
  '/matching': 'nav.group.allocation',
  '/conflicts': 'nav.group.allocation',
  '/simulations': 'nav.group.planning',
  '/capacity': 'nav.group.planning',
  '/financials': 'nav.group.finance',
}

export function getPageTitleKey(pathname: string): MessageKey {
  return routePageKeys[pathname] ?? 'pages.dashboard.title'
}

export function getGroupKey(pathname: string): MessageKey {
  return routeGroupKeys[pathname] ?? 'nav.group.overview'
}

const routeDescriptionKeys: Record<string, MessageKey> = {
  '/': 'pages.dashboard.description',
  '/skills': 'pages.skills.description',
  '/people': 'pages.people.description',
  '/projects': 'pages.projects.description',
  '/allocation-needs': 'pages.needs.description',
  '/allocations': 'pages.allocations.description',
  '/simulations': 'pages.simulations.description',
  '/capacity': 'pages.capacity.description',
  '/matching': 'pages.matching.description',
  '/financials': 'pages.financials.description',
  '/conflicts': 'pages.conflicts.description',
}

export function getPageDescriptionKey(pathname: string): MessageKey | undefined {
  return routeDescriptionKeys[pathname]
}

export function isNavItemActive(item: NavItem, pathname: string): boolean {
  if (item.end ?? item.to === '/') {
    return pathname === item.to
  }
  return pathname === item.to || pathname.startsWith(`${item.to}/`)
}

export function isNavGroupActive(group: NavGroup, pathname: string): boolean {
  return group.items.some((item) => isNavItemActive(item, pathname))
}

export const hiddenNavPaths = ['/simulations', '/financials'] as const

export type HiddenNavPath = (typeof hiddenNavPaths)[number]

export function isHiddenNavPath(path: string): path is HiddenNavPath {
  return (hiddenNavPaths as readonly string[]).includes(path)
}

export function filterNavGroups(groups: NavGroup[], showHiddenOptions: boolean): NavGroup[] {
  if (showHiddenOptions) return groups

  return groups
    .map((group) => ({
      ...group,
      items: group.items.filter((item) => !isHiddenNavPath(item.to)),
    }))
    .filter((group) => group.items.length > 0)
}

export function getVisibleNavGroups(showHiddenOptions: boolean): NavGroup[] {
  return filterNavGroups(navGroups, showHiddenOptions)
}

export function getActiveNavGroup(pathname: string, showHiddenOptions = true): NavGroup {
  const groups = getVisibleNavGroups(showHiddenOptions)
  return groups.find((group) => isNavGroupActive(group, pathname)) ?? groups[0]
}
