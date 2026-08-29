import type { Locale } from './messages'

/** Chaves i18n das páginas Matching, Simulations, Financials e Allocations */
export type PageMessageKey =
  | 'common.person'
  | 'common.revenue'
  | 'common.cost'
  | 'common.margin'
  | 'common.hours'
  | 'common.quantity'
  | 'common.any'
  | 'common.simulate'
  | 'alloc.tenantRequired'
  | 'alloc.loadListsError'
  | 'alloc.new'
  | 'alloc.workload.title'
  | 'alloc.workload.empty'
  | 'alloc.workload.peak'
  | 'alloc.workload.remaining'
  | 'alloc.workload.existing'
  | 'alloc.workload.overload'
  | 'alloc.workload.closedNote'
  | 'empty.allocations'
  | 'matching.selectNeed'
  | 'matching.selectOpenNeed'
  | 'matching.minAvailability'
  | 'matching.minAvailabilityPlaceholder'
  | 'matching.excludeOnProject'
  | 'matching.noOpenNeeds'
  | 'matching.selectNeedPrompt'
  | 'matching.plannedAllocations'
  | 'matching.rankedCandidates'
  | 'matching.noCandidates'
  | 'matching.rejected'
  | 'matching.history'
  | 'matching.decisionAccepted'
  | 'matching.decisionRejected'
  | 'matching.projectOverview'
  | 'matching.projectOverviewHint'
  | 'matching.openInMatching'
  | 'matching.noCandidatesBatch'
  | 'matching.calculating'
  | 'matching.onProject'
  | 'matching.rejectedBadge'
  | 'matching.viewAllocations'
  | 'matching.allocate'
  | 'matching.allocating'
  | 'matching.reject'
  | 'matching.saving'
  | 'matching.reconsider'
  | 'matching.minAvail'
  | 'matching.scoreLabel'
  | 'matching.feedback.allocated'
  | 'matching.feedback.rejected'
  | 'matching.error.overload'
  | 'matching.error.inactive'
  | 'matching.score.availability'
  | 'matching.score.requiredSkills'
  | 'matching.score.desiredSkills'
  | 'matching.score.seniority'
  | 'matching.score.history'
  | 'matching.score.cost'
  | 'matching.score.overload'
  | 'matching.score.switching'
  | 'matching.requiredDedication'
  | 'sim.scenario'
  | 'sim.desiredStart'
  | 'sim.durationMonths'
  | 'sim.requiredRoles'
  | 'sim.addRole'
  | 'sim.simulate'
  | 'sim.analyzing'
  | 'sim.emptyPrompt'
  | 'sim.emptyPromptBold'
  | 'sim.verdict'
  | 'sim.feasible'
  | 'sim.notFeasible'
  | 'sim.canStartOn'
  | 'sim.notFeasibleOn'
  | 'sim.earliestEstimate'
  | 'sim.activePeople'
  | 'sim.onBench'
  | 'sim.gapRoles'
  | 'sim.requestedRoles'
  | 'sim.simulatedPeriod'
  | 'sim.anySeniority'
  | 'sim.dedicationShort'
  | 'sim.ok'
  | 'sim.bottleneck'
  | 'sim.eligibleCandidates'
  | 'sim.noEligible'
  | 'sim.covered'
  | 'sim.missing'
  | 'financials.marginAlertTitle'
  | 'financials.costLabel'
  | 'financials.projectsSection'
  | 'financials.revenue'
  | 'financials.profitability'
  | 'financials.byClient'
  | 'financials.byProjectType'
  | 'financials.noDataInPeriod'
  | 'financials.projectCount'
  | 'financials.groupType'
  | 'financials.projectDetail'
  | 'financials.selectProject'
  | 'financials.loadingDetail'
  | 'financials.estimatedRevenue'
  | 'financials.periodCost'
  | 'financials.noAllocationsInPeriod'
  | 'financials.benchCost'
  | 'financials.benchCostDesc'
  | 'financials.nobodyOnBench'
  | 'financials.marginSimulation'
  | 'financials.currentMargin'
  | 'financials.projectedMargin'
  | 'financials.additionalCost'
  | 'financials.marginAlertThreshold'
  | 'alloc.guide.step1'
  | 'alloc.guide.step2'
  | 'alloc.guide.step3'
  | 'alloc.rn001Tip'
  | 'alloc.hero.label'
  | 'alloc.hero.hint'
  | 'alloc.hero.planned'
  | 'alloc.hero.confirmed'
  | 'matching.guide.step1'
  | 'matching.guide.step2'
  | 'matching.guide.step3'
  | 'matching.hero.label'
  | 'matching.hero.hint'
  | 'matching.hero.openNeeds'
  | 'matching.hero.topScore'
  | 'sim.guide.step1'
  | 'sim.guide.step2'
  | 'sim.guide.step3'
  | 'sim.hero.label'
  | 'sim.hero.hint'
  | 'sim.hero.roles'
  | 'financials.guide.step1'
  | 'financials.guide.step2'
  | 'financials.guide.step3'
  | 'financials.hero.label'
  | 'financials.hero.hint'
  | 'financials.hero.marginPercent'
  | 'financials.hero.benchCost'
  | 'financials.donut.revenue'
  | 'financials.donut.cost'
  | 'financials.donut.margin'
  | 'financials.donut.title'
  | 'financials.donut.subtitle'

const pt: Record<PageMessageKey, string> = {
  'common.person': 'Pessoa',
  'common.revenue': 'Receita',
  'common.cost': 'Custo',
  'common.margin': 'Margem',
  'common.hours': 'Horas',
  'common.quantity': 'Qtd',
  'common.any': 'Qualquer',
  'common.simulate': 'Simular',
  'alloc.tenantRequired': 'Configure um Tenant ID válido para carregar pessoas e projetos.',
  'alloc.loadListsError': 'Erro ao carregar listas.',
  'alloc.new': 'Nova alocação',
  'alloc.workload.title': 'Carga da pessoa no período',
  'alloc.workload.empty': 'Nenhuma alocação ativa neste período — até 100% disponível por semana.',
  'alloc.workload.peak': 'Pico na semana {start} → {end}: {allocated}% já alocados',
  'alloc.workload.remaining': 'Sobra até {remaining}% nesta semana (RN-001).',
  'alloc.workload.existing': 'Alocações ativas que cruzam o período',
  'alloc.workload.overload':
    'Esta dedicação ultrapassaria 100% na semana {start} → {end}: já há {existing}% + {requested}% solicitados = {total}%.',
  'alloc.workload.closedNote': 'Alocações encerradas (Closed) não entram na conta.',
  'empty.allocations': 'Nenhuma alocação cadastrada.',
  'matching.selectNeed': 'Necessidade de alocação',
  'matching.selectOpenNeed': 'Selecione uma necessidade aberta…',
  'matching.minAvailability': 'Disp. mínima %',
  'matching.minAvailabilityPlaceholder': 'Qualquer',
  'matching.excludeOnProject': 'Excluir quem já está no projeto',
  'matching.noOpenNeeds': 'Nenhuma necessidade aberta. Crie uma em',
  'matching.selectNeedPrompt': 'Selecione uma necessidade para ver candidatos ranqueados pelo score RN-007.',
  'matching.plannedAllocations': 'Alocações planejadas ({count})',
  'matching.rankedCandidates': 'Candidatos ranqueados ({count})',
  'matching.noCandidates':
    'Nenhum candidato encontrado com os filtros atuais. Tente reduzir a disp. mínima ou desmarcar "Excluir quem já está no projeto".',
  'matching.rejected': 'Rejeitados ({count})',
  'matching.history': 'Histórico de decisões',
  'matching.decisionAccepted': 'Aceito',
  'matching.decisionRejected': 'Rejeitado',
  'matching.projectOverview': 'Visão rápida por projeto',
  'matching.projectOverviewHint':
    'Top 5 candidatos por necessidade aberta — somente leitura. Use o seletor acima para alocar como Planejada.',
  'matching.openInMatching': 'Abrir no matching',
  'matching.noCandidatesBatch': 'Sem candidatos.',
  'matching.calculating': 'Calculando…',
  'matching.onProject': 'Já no projeto',
  'matching.rejectedBadge': 'Rejeitado',
  'matching.viewAllocations': 'Ver em Alocações',
  'matching.allocate': 'Alocar',
  'matching.allocating': 'Alocando…',
  'matching.reject': 'Rejeitar',
  'matching.saving': 'Salvando…',
  'matching.reconsider': 'Reconsiderar',
  'matching.minAvail': 'Disp. mín.',
  'matching.scoreLabel': 'Score',
  'matching.feedback.allocated':
    '{name} alocado(a) como Planejada. Confirme ou ajuste em Alocações quando quiser.',
  'matching.feedback.rejected': '{name} rejeitado(a). Você pode reconsiderar depois.',
  'matching.error.overload': 'Não foi possível alocar: esta pessoa ficaria superalocada no período.',
  'matching.error.inactive': 'Não foi possível alocar: pessoa inativa.',
  'matching.score.availability': 'Disponibilidade',
  'matching.score.requiredSkills': 'Skills obrig.',
  'matching.score.desiredSkills': 'Skills desej.',
  'matching.score.seniority': 'Senioridade',
  'matching.score.history': 'Histórico',
  'matching.score.cost': 'Custo',
  'matching.score.overload': 'Superaloc.',
  'matching.score.switching': 'Troca proj.',
  'matching.requiredDedication': 'Dedicação necessária',
  'sim.scenario': 'Cenário',
  'sim.desiredStart': 'Início desejado',
  'sim.durationMonths': 'Duração (meses)',
  'sim.requiredRoles': 'Papéis necessários',
  'sim.addRole': '+ Papel',
  'sim.simulate': 'Simular cenário',
  'sim.analyzing': 'Analisando capacidade do tenant…',
  'sim.emptyPrompt': 'Configure o cenário à esquerda e clique em',
  'sim.emptyPromptBold': 'Simular cenário',
  'sim.verdict': 'Veredito',
  'sim.feasible': 'Projeto viável',
  'sim.notFeasible': 'Capacidade insuficiente',
  'sim.canStartOn': 'Pode iniciar em {date}',
  'sim.notFeasibleOn': 'Não viável em {date}',
  'sim.earliestEstimate': ' — estimativa: {date} (+{weeks} sem.)',
  'sim.activePeople': 'Pessoas ativas',
  'sim.onBench': 'No bench',
  'sim.gapRoles': 'Vagas em falta',
  'sim.requestedRoles': 'Vagas pedidas',
  'sim.simulatedPeriod': 'Período simulado: {from} → {to}',
  'sim.anySeniority': 'Qualquer senioridade',
  'sim.dedicationShort': '{percent}% dedicação',
  'sim.ok': 'OK',
  'sim.bottleneck': 'Gargalo',
  'sim.eligibleCandidates': 'Quem pode entrar',
  'sim.noEligible': 'Nenhuma pessoa atende senioridade, skills e disponibilidade para este papel.',
  'sim.covered': 'Coberto',
  'sim.missing': 'Faltam {count}',
  'financials.marginAlertTitle': 'Projetos com margem baixa (<{threshold}%)',
  'financials.costLabel': 'custo',
  'financials.projectsSection': 'Projetos',
  'financials.revenue': 'Receita',
  'financials.profitability': 'Rentabilidade agregada',
  'financials.byClient': 'Por cliente',
  'financials.byProjectType': 'Por tipo de projeto',
  'financials.noDataInPeriod': 'Sem dados no período.',
  'financials.projectCount': 'Projetos',
  'financials.groupType': 'Tipo',
  'financials.projectDetail': 'Detalhe por projeto',
  'financials.selectProject': 'Selecione um projeto…',
  'financials.loadingDetail': 'Carregando detalhes…',
  'financials.estimatedRevenue': 'Receita estimada',
  'financials.periodCost': 'Custo no período',
  'financials.noAllocationsInPeriod': 'Sem alocações no período.',
  'financials.benchCost': 'Custo de bench',
  'financials.benchCostDesc': 'Custo da capacidade ociosa (≥50% disponível):',
  'financials.nobodyOnBench': 'Ninguém em bench no período.',
  'financials.marginSimulation': 'Simular margem antes de alocar',
  'financials.currentMargin': 'Margem atual',
  'financials.projectedMargin': 'Margem projetada',
  'financials.additionalCost': 'Custo adicional',
  'financials.marginAlertThreshold': 'Alerta margem <',
  'alloc.guide.step1': 'Uma alocação liga pessoa ↔ projeto com dedicação e período.',
  'alloc.guide.step2': 'Status Planejada pode vir do matching; Confirmed é operacional.',
  'alloc.guide.step3': 'RN-001 impede salvar se a pessoa passar de 100% na mesma semana.',
  'alloc.rn001Tip':
    'RN-001: a soma da dedicação (%) da pessoa em todas as alocações ativas na mesma semana (segunda a domingo) não pode ultrapassar 100%. Se ultrapassar, o salvamento é bloqueado com erro de superalocação.',
  'alloc.hero.label': 'Alocações ativas',
  'alloc.hero.hint': 'Vínculos que consomem capacidade da equipe.',
  'alloc.hero.planned': 'Planejadas',
  'alloc.hero.confirmed': 'Confirmadas',
  'matching.guide.step1': 'Escolha uma necessidade aberta para ver candidatos ranqueados.',
  'matching.guide.step2': 'O score RN-007 combina disponibilidade, skills, senioridade e custo.',
  'matching.guide.step3': 'Alocar cria uma alocação Planejada — confirme em Alocações.',
  'matching.hero.label': 'Necessidades abertas',
  'matching.hero.hint': 'Vagas aguardando candidato ou decisão.',
  'matching.hero.openNeeds': 'Abertas',
  'matching.hero.topScore': 'Melhor score',
  'sim.guide.step1': 'Monte um cenário comercial: papéis, dedicação e skills.',
  'sim.guide.step2': 'A simulação verifica se há gente disponível no período.',
  'sim.guide.step3': 'Use antes de vender — não cria alocações reais.',
  'sim.hero.label': 'Papéis no cenário',
  'sim.hero.hint': 'Configure à esquerda e simule a viabilidade.',
  'sim.hero.roles': 'Papéis',
  'financials.guide.step1': 'Visão de receita, custo e margem no período filtrado.',
  'financials.guide.step2': 'Alertas destacam projetos com margem abaixo do limiar.',
  'financials.guide.step3': 'Bench mostra custo de capacidade ociosa em R$.',
  'financials.hero.label': 'Margem no período',
  'financials.hero.hint': 'Resultado agregado de receita menos custo alocado.',
  'financials.hero.marginPercent': 'Margem %',
  'financials.hero.benchCost': 'Custo bench',
  'financials.donut.revenue': 'Receita',
  'financials.donut.cost': 'Custo',
  'financials.donut.margin': 'Margem',
  'financials.donut.title': 'Composição financeira',
  'financials.donut.subtitle': 'Distribuição de receita, custo e margem no período.',
}

const en: Record<PageMessageKey, string> = {
  'common.person': 'Person',
  'common.revenue': 'Revenue',
  'common.cost': 'Cost',
  'common.margin': 'Margin',
  'common.hours': 'Hours',
  'common.quantity': 'Qty',
  'common.any': 'Any',
  'common.simulate': 'Simulate',
  'alloc.tenantRequired': 'Set a valid Tenant ID to load people and projects.',
  'alloc.loadListsError': 'Failed to load lists.',
  'alloc.new': 'New allocation',
  'alloc.workload.title': 'Person load in this period',
  'alloc.workload.empty': 'No active allocations in this period — up to 100% available per week.',
  'alloc.workload.peak': 'Peak week {start} → {end}: {allocated}% already allocated',
  'alloc.workload.remaining': 'Up to {remaining}% left this week (RN-001).',
  'alloc.workload.existing': 'Active allocations overlapping this period',
  'alloc.workload.overload':
    'This dedication would exceed 100% in week {start} → {end}: {existing}% already + {requested}% requested = {total}%.',
  'alloc.workload.closedNote': 'Closed allocations are not counted.',
  'empty.allocations': 'No allocations registered.',
  'matching.selectNeed': 'Allocation need',
  'matching.selectOpenNeed': 'Select an open need…',
  'matching.minAvailability': 'Min. availability %',
  'matching.minAvailabilityPlaceholder': 'Any',
  'matching.excludeOnProject': 'Exclude people already on the project',
  'matching.noOpenNeeds': 'No open needs. Create one in',
  'matching.selectNeedPrompt': 'Select a need to see candidates ranked by RN-007 score.',
  'matching.plannedAllocations': 'Planned allocations ({count})',
  'matching.rankedCandidates': 'Ranked candidates ({count})',
  'matching.noCandidates':
    'No candidates found with current filters. Try lowering min. availability or uncheck "Exclude people already on the project".',
  'matching.rejected': 'Rejected ({count})',
  'matching.history': 'Decision history',
  'matching.decisionAccepted': 'Accepted',
  'matching.decisionRejected': 'Rejected',
  'matching.projectOverview': 'Quick view by project',
  'matching.projectOverviewHint':
    'Top 5 candidates per open need — read only. Use the selector above to allocate as Planned.',
  'matching.openInMatching': 'Open in matching',
  'matching.noCandidatesBatch': 'No candidates.',
  'matching.calculating': 'Calculating…',
  'matching.onProject': 'Already on project',
  'matching.rejectedBadge': 'Rejected',
  'matching.viewAllocations': 'View in Allocations',
  'matching.allocate': 'Allocate',
  'matching.allocating': 'Allocating…',
  'matching.reject': 'Reject',
  'matching.saving': 'Saving…',
  'matching.reconsider': 'Reconsider',
  'matching.minAvail': 'Min. avail.',
  'matching.scoreLabel': 'Score',
  'matching.feedback.allocated':
    '{name} allocated as Planned. Confirm or adjust in Allocations when ready.',
  'matching.feedback.rejected': '{name} rejected. You can reconsider later.',
  'matching.error.overload': 'Could not allocate: person would be overallocated in this period.',
  'matching.error.inactive': 'Could not allocate: inactive person.',
  'matching.score.availability': 'Availability',
  'matching.score.requiredSkills': 'Req. skills',
  'matching.score.desiredSkills': 'Desired skills',
  'matching.score.seniority': 'Seniority',
  'matching.score.history': 'History',
  'matching.score.cost': 'Cost',
  'matching.score.overload': 'Overload',
  'matching.score.switching': 'Proj. switch',
  'matching.requiredDedication': 'Required dedication',
  'sim.scenario': 'Scenario',
  'sim.desiredStart': 'Desired start',
  'sim.durationMonths': 'Duration (months)',
  'sim.requiredRoles': 'Required roles',
  'sim.addRole': '+ Role',
  'sim.simulate': 'Simulate scenario',
  'sim.analyzing': 'Analyzing tenant capacity…',
  'sim.emptyPrompt': 'Configure the scenario on the left and click',
  'sim.emptyPromptBold': 'Simulate scenario',
  'sim.verdict': 'Verdict',
  'sim.feasible': 'Project feasible',
  'sim.notFeasible': 'Insufficient capacity',
  'sim.canStartOn': 'Can start on {date}',
  'sim.notFeasibleOn': 'Not feasible on {date}',
  'sim.earliestEstimate': ' — estimate: {date} (+{weeks} wk.)',
  'sim.activePeople': 'Active people',
  'sim.onBench': 'On bench',
  'sim.gapRoles': 'Roles short',
  'sim.requestedRoles': 'Roles requested',
  'sim.simulatedPeriod': 'Simulated period: {from} → {to}',
  'sim.anySeniority': 'Any seniority',
  'sim.dedicationShort': '{percent}% dedication',
  'sim.ok': 'OK',
  'sim.bottleneck': 'Bottleneck',
  'sim.eligibleCandidates': 'Who can join',
  'sim.noEligible': 'No person matches seniority, skills, and availability for this role.',
  'sim.covered': 'Covered',
  'sim.missing': 'Missing {count}',
  'financials.marginAlertTitle': 'Projects with low margin (<{threshold}%)',
  'financials.costLabel': 'cost',
  'financials.projectsSection': 'Projects',
  'financials.revenue': 'Revenue',
  'financials.profitability': 'Aggregated profitability',
  'financials.byClient': 'By client',
  'financials.byProjectType': 'By project type',
  'financials.noDataInPeriod': 'No data in period.',
  'financials.projectCount': 'Projects',
  'financials.groupType': 'Type',
  'financials.projectDetail': 'Project detail',
  'financials.selectProject': 'Select a project…',
  'financials.loadingDetail': 'Loading details…',
  'financials.estimatedRevenue': 'Estimated revenue',
  'financials.periodCost': 'Cost in period',
  'financials.noAllocationsInPeriod': 'No allocations in period.',
  'financials.benchCost': 'Bench cost',
  'financials.benchCostDesc': 'Idle capacity cost (≥50% available):',
  'financials.nobodyOnBench': 'Nobody on bench in period.',
  'financials.marginSimulation': 'Simulate margin before allocating',
  'financials.currentMargin': 'Current margin',
  'financials.projectedMargin': 'Projected margin',
  'financials.additionalCost': 'Additional cost',
  'financials.marginAlertThreshold': 'Margin alert <',
  'alloc.guide.step1': 'An allocation links person ↔ project with dedication and period.',
  'alloc.guide.step2': 'Planned status may come from matching; Confirmed is operational.',
  'alloc.guide.step3': 'RN-001 blocks save if the person exceeds 100% in the same week.',
  'alloc.rn001Tip':
    'RN-001: the sum of a person’s dedication (%) across all active allocations in the same week (Monday–Sunday) cannot exceed 100%. If it does, save is blocked with an overallocation error.',
  'alloc.hero.label': 'Active allocations',
  'alloc.hero.hint': 'Links that consume team capacity.',
  'alloc.hero.planned': 'Planned',
  'alloc.hero.confirmed': 'Confirmed',
  'matching.guide.step1': 'Pick an open need to see ranked candidates.',
  'matching.guide.step2': 'RN-007 score combines availability, skills, seniority, and cost.',
  'matching.guide.step3': 'Allocate creates a Planned allocation — confirm in Allocations.',
  'matching.hero.label': 'Open needs',
  'matching.hero.hint': 'Roles waiting for a candidate or decision.',
  'matching.hero.openNeeds': 'Open',
  'matching.hero.topScore': 'Top score',
  'sim.guide.step1': 'Build a sales scenario: roles, dedication, and skills.',
  'sim.guide.step2': 'Simulation checks if people are available in the period.',
  'sim.guide.step3': 'Use before selling — does not create real allocations.',
  'sim.hero.label': 'Roles in scenario',
  'sim.hero.hint': 'Configure on the left and simulate feasibility.',
  'sim.hero.roles': 'Roles',
  'financials.guide.step1': 'Revenue, cost, and margin view for the filtered period.',
  'financials.guide.step2': 'Alerts highlight projects below the margin threshold.',
  'financials.guide.step3': 'Bench shows idle capacity cost in currency.',
  'financials.hero.label': 'Margin in period',
  'financials.hero.hint': 'Aggregate result of revenue minus allocated cost.',
  'financials.hero.marginPercent': 'Margin %',
  'financials.hero.benchCost': 'Bench cost',
  'financials.donut.revenue': 'Revenue',
  'financials.donut.cost': 'Cost',
  'financials.donut.margin': 'Margin',
  'financials.donut.title': 'Financial composition',
  'financials.donut.subtitle': 'Revenue, cost, and margin split for the period.',
}

export const pageMessages: Record<Locale, Record<PageMessageKey, string>> = { pt, en }

export function translatePage(
  locale: Locale,
  key: PageMessageKey,
  vars?: Record<string, string | number>,
): string {
  let text = pageMessages[locale][key] ?? key
  if (vars) {
    for (const [k, v] of Object.entries(vars)) {
      text = text.replace(new RegExp(`\\{${k}\\}`, 'g'), String(v))
    }
  }
  return text
}
