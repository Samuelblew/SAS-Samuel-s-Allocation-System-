import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Line,
  LineChart,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import type { CapacitySlice } from '../../lib/capacityMetrics'
import type { TeamOccupation, WeekCapacityGap, WeekOverview } from '../../lib/types'
import { formatPercent } from '../../lib/dates'
import { CAPACITY_CHART_COLORS, CHART_TOOLTIP_STYLE } from './chartTheme'

interface SliceLabel {
  id: CapacitySlice['id']
  label: string
  color: string
}

export function CapacityDonutChart({
  slices,
  labels,
  centerValue,
  centerLabel,
}: {
  slices: CapacitySlice[]
  labels: SliceLabel[]
  centerValue: string
  centerLabel: string
}) {
  const labelById = Object.fromEntries(labels.map((item) => [item.id, item])) as Record<
    CapacitySlice['id'],
    SliceLabel
  >
  const data = slices.map((slice) => ({
    ...slice,
    name: labelById[slice.id]?.label ?? slice.id,
    fill: labelById[slice.id]?.color ?? CAPACITY_CHART_COLORS.allocated,
  }))

  if (data.length === 0) {
    return <p className="text-sm ias-text-subtle">{centerLabel}</p>
  }

  return (
    <div className="capacity-donut">
      <div className="capacity-donut__chart">
        <ResponsiveContainer width="100%" height={240}>
          <PieChart>
            <Pie
              data={data}
              dataKey="value"
              nameKey="name"
              cx="50%"
              cy="50%"
              innerRadius={68}
              outerRadius={96}
              paddingAngle={2}
              stroke="transparent"
            >
              {data.map((entry) => (
                <Cell key={entry.id} fill={entry.fill} />
              ))}
            </Pie>
            <Tooltip
              formatter={(value, _name, item) => {
                const hours = Number(value ?? 0)
                const percent = (item?.payload as { percent?: number })?.percent ?? 0
                return [`${hours.toFixed(0)}h (${percent}%)`, item?.name ?? '']
              }}
              contentStyle={CHART_TOOLTIP_STYLE}
            />
          </PieChart>
        </ResponsiveContainer>
        <div className="capacity-donut__center" aria-hidden>
          <span className="capacity-donut__center-value">{centerValue}</span>
          <span className="capacity-donut__center-label">{centerLabel}</span>
        </div>
      </div>
      <ul className="capacity-legend">
        {data.map((entry) => (
          <li key={entry.id} className="capacity-legend__item">
            <span className="capacity-legend__swatch" style={{ backgroundColor: entry.fill }} />
            <span className="capacity-legend__label">{entry.name}</span>
            <span className="capacity-legend__value">{entry.percent}%</span>
          </li>
        ))}
      </ul>
    </div>
  )
}

export function CapacityTrendChart({
  weeks,
  weekLabels,
  allocatedLabel,
  availableLabel,
}: {
  weeks: WeekOverview[]
  weekLabels: string[]
  allocatedLabel: string
  availableLabel: string
}) {
  const data = weeks.map((week, index) => ({
    label: weekLabels[index],
    allocated: Math.round(week.avgAllocatedPercent * 10) / 10,
    available: Math.round(week.avgAvailablePercent * 10) / 10,
  }))

  return (
    <ResponsiveContainer width="100%" height={260}>
      <LineChart data={data} margin={{ top: 8, right: 8, left: -16, bottom: 0 }}>
        <CartesianGrid stroke="var(--ias-border-strong)" strokeDasharray="4 4" vertical={false} />
        <XAxis
          dataKey="label"
          tick={{ fill: 'var(--ias-text-subtle)', fontSize: 11 }}
          axisLine={false}
          tickLine={false}
        />
        <YAxis
          domain={[0, 100]}
          tick={{ fill: 'var(--ias-text-subtle)', fontSize: 11 }}
          axisLine={false}
          tickLine={false}
          tickFormatter={(value) => `${value}%`}
        />
        <Tooltip
          formatter={(value) => `${Number(value ?? 0)}%`}
          contentStyle={CHART_TOOLTIP_STYLE}
        />
        <Legend wrapperStyle={{ fontSize: '12px', color: 'var(--ias-text-muted)' }} />
        <Line
          type="monotone"
          dataKey="allocated"
          name={allocatedLabel}
          stroke={CAPACITY_CHART_COLORS.allocated}
          strokeWidth={2.5}
          dot={{ r: 3 }}
          activeDot={{ r: 5 }}
        />
        <Line
          type="monotone"
          dataKey="available"
          name={availableLabel}
          stroke={CAPACITY_CHART_COLORS.available}
          strokeWidth={2.5}
          dot={{ r: 3 }}
          activeDot={{ r: 5 }}
        />
      </LineChart>
    </ResponsiveContainer>
  )
}

export function CapacityHoursChart({
  weeks,
  weekLabels,
  allocatedLabel,
  availableLabel,
  unavailableLabel,
}: {
  weeks: WeekOverview[]
  weekLabels: string[]
  allocatedLabel: string
  availableLabel: string
  unavailableLabel: string
}) {
  const data = weeks.map((week, index) => {
    const unavailable = Math.max(
      0,
      week.totalCapacityHours - week.totalAllocatedHours - week.totalAvailableHours,
    )
    return {
      label: weekLabels[index],
      allocated: week.totalAllocatedHours,
      available: week.totalAvailableHours,
      unavailable,
    }
  })

  return (
    <ResponsiveContainer width="100%" height={260}>
      <BarChart data={data} margin={{ top: 8, right: 8, left: -8, bottom: 0 }}>
        <CartesianGrid stroke="var(--ias-border-strong)" strokeDasharray="4 4" vertical={false} />
        <XAxis
          dataKey="label"
          tick={{ fill: 'var(--ias-text-subtle)', fontSize: 11 }}
          axisLine={false}
          tickLine={false}
        />
        <YAxis
          tick={{ fill: 'var(--ias-text-subtle)', fontSize: 11 }}
          axisLine={false}
          tickLine={false}
          tickFormatter={(value) => `${value}h`}
        />
        <Tooltip
          formatter={(value) => `${Number(value ?? 0).toFixed(0)}h`}
          contentStyle={CHART_TOOLTIP_STYLE}
        />
        <Legend wrapperStyle={{ fontSize: '12px', color: 'var(--ias-text-muted)' }} />
        <Bar
          dataKey="allocated"
          name={allocatedLabel}
          stackId="hours"
          fill={CAPACITY_CHART_COLORS.allocated}
          radius={[0, 0, 0, 0]}
        />
        <Bar
          dataKey="available"
          name={availableLabel}
          stackId="hours"
          fill={CAPACITY_CHART_COLORS.available}
        />
        <Bar
          dataKey="unavailable"
          name={unavailableLabel}
          stackId="hours"
          fill={CAPACITY_CHART_COLORS.unavailable}
          radius={[4, 4, 0, 0]}
        />
      </BarChart>
    </ResponsiveContainer>
  )
}

export function CapacityGapChart({
  weeks,
  weekLabels,
  demandLabel,
  supplyLabel,
}: {
  weeks: WeekCapacityGap[]
  weekLabels: string[]
  demandLabel: string
  supplyLabel: string
}) {
  const data = weeks.map((week, index) => ({
    label: weekLabels[index],
    demand: week.totalGapDemandPercent,
    supply: week.totalAvailableSupplyPercent,
    shortfall: week.netShortfallPercent,
  }))

  return (
    <ResponsiveContainer width="100%" height={240}>
      <BarChart data={data} margin={{ top: 8, right: 8, left: -16, bottom: 0 }}>
        <CartesianGrid stroke="var(--ias-border-strong)" strokeDasharray="4 4" vertical={false} />
        <XAxis
          dataKey="label"
          tick={{ fill: 'var(--ias-text-subtle)', fontSize: 11 }}
          axisLine={false}
          tickLine={false}
        />
        <YAxis
          tick={{ fill: 'var(--ias-text-subtle)', fontSize: 11 }}
          axisLine={false}
          tickLine={false}
          tickFormatter={(value) => `${value}%`}
        />
        <Tooltip
          formatter={(value, name) => {
            const label = name === 'demand' ? demandLabel : supplyLabel
            return [`${Number(value ?? 0).toFixed(1)}%`, label]
          }}
          contentStyle={CHART_TOOLTIP_STYLE}
        />
        <Legend wrapperStyle={{ fontSize: '12px', color: 'var(--ias-text-muted)' }} />
        <Bar dataKey="demand" name={demandLabel} fill={CAPACITY_CHART_COLORS.demand} radius={[4, 4, 0, 0]} />
        <Bar dataKey="supply" name={supplyLabel} fill={CAPACITY_CHART_COLORS.supply} radius={[4, 4, 0, 0]} />
      </BarChart>
    </ResponsiveContainer>
  )
}

export function CapacityBarList({
  items,
}: {
  items: { id: string; label: string; value: number; hint?: string }[]
}) {
  if (items.length === 0) {
    return null
  }

  const max = Math.max(...items.map((item) => item.value), 1)

  return (
    <ul className="capacity-bar-list">
      {items.map((item) => (
        <li key={item.id} className="capacity-bar-list__item">
          <div className="capacity-bar-list__header">
            <span className="capacity-bar-list__label">{item.label}</span>
            <span className="capacity-bar-list__value">{formatPercent(item.value)}</span>
          </div>
          <div className="capacity-bar-list__track" aria-hidden>
            <div
              className="capacity-bar-list__fill"
              style={{ width: `${Math.min(100, (item.value / max) * 100)}%` }}
            />
          </div>
          {item.hint ? <p className="capacity-bar-list__hint">{item.hint}</p> : null}
        </li>
      ))}
    </ul>
  )
}

export function CapacityTeamBars({
  teams,
  peopleLabel,
  noTeamLabel,
}: {
  teams: TeamOccupation[]
  peopleLabel: string
  noTeamLabel: string
}) {
  return (
    <CapacityBarList
      items={teams.map((team) => ({
        id: team.team ?? '_',
        label: team.team ?? noTeamLabel,
        value: team.avgAllocatedPercent,
        hint: `${team.peopleCount} ${peopleLabel}`,
      }))}
    />
  )
}
