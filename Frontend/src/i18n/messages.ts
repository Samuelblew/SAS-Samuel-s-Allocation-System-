export type Locale = 'pt' | 'en'

export type MessageKey =
  | 'app.name'
  | 'app.tagline'
  | 'nav.dashboard'
  | 'nav.skills'
  | 'nav.people'
  | 'nav.projects'
  | 'nav.needs'
  | 'nav.allocations'
  | 'nav.simulations'
  | 'nav.capacity'
  | 'nav.matching'
  | 'nav.financials'
  | 'nav.conflicts'
  | 'layout.tenant'
  | 'layout.actor'
  | 'layout.tenantPlaceholder'
  | 'layout.actorPlaceholder'
  | 'layout.tenantInvalid'
  | 'layout.themeLight'
  | 'layout.themeDark'
  | 'layout.devConfig'
  | 'layout.workspace'
  | 'layout.tenantReady'
  | 'layout.tenantPending'
  | 'layout.api'
  | 'layout.apiChecking'
  | 'layout.apiDown'
  | 'layout.hiddenOptions'
  | 'layout.menu'
  | 'layout.navMobile'
  | 'layout.preferences'
  | 'layout.language'
  | 'nav.group.overview'
  | 'nav.group.team'
  | 'nav.group.allocation'
  | 'nav.group.planning'
  | 'nav.group.finance'
  | 'common.loading'
  | 'common.save'
  | 'common.cancel'
  | 'common.edit'
  | 'common.delete'
  | 'common.select'
  | 'common.from'
  | 'common.to'
  | 'common.yes'
  | 'common.no'
  | 'common.add'
  | 'common.actions'
  | 'common.name'
  | 'common.category'
  | 'common.none'
  | 'table.skillSingular'
  | 'table.skillPlural'
  | 'common.confirmDelete'
  | 'common.close'
  | 'common.status'
  | 'common.priority'
  | 'common.period'
  | 'common.skills'
  | 'common.role'
  | 'common.details'
  | 'common.hide'
  | 'common.remove'
  | 'common.client'
  | 'common.project'
  | 'common.jobTitle'
  | 'common.seniority'
  | 'common.weeklyHours'
  | 'common.dedication'
  | 'common.urgency'
  | 'common.criticality'
  | 'common.startDate'
  | 'common.endDate'
  | 'form.skills.new'
  | 'form.skills.edit'
  | 'form.people.new'
  | 'form.people.edit'
  | 'form.people.skillsOf'
  | 'form.people.noSkills'
  | 'form.people.assignSkill'
  | 'form.people.skillLevel'
  | 'form.projects.newClient'
  | 'form.projects.new'
  | 'form.projects.edit'
  | 'form.projects.createClient'
  | 'form.projects.create'
  | 'form.projects.registeredClients'
  | 'form.projects.clientCount'
  | 'form.projects.clientHasProjects'
  | 'form.needs.editBanner'
  | 'form.needs.requiredSkills'
  | 'form.needs.skillsSelected'
  | 'form.needs.noRequiredSkills'
  | 'form.needs.create'
  | 'form.needs.saveChanges'
  | 'empty.skills'
  | 'empty.people'
  | 'empty.projects'
  | 'empty.needs'
  | 'empty.conflicts'
  | 'empty.generic'
  | 'capacity.weeksInPeriod'
  | 'capacity.avgAllocated'
  | 'capacity.lastWeekHours'
  | 'capacity.benchCount'
  | 'capacity.utilizationRate'
  | 'capacity.utilizationHint'
  | 'capacity.howItWorks.title'
  | 'capacity.howItWorks.step1'
  | 'capacity.howItWorks.step2'
  | 'capacity.howItWorks.step3'
  | 'capacity.avgAvailable'
  | 'capacity.openRoles'
  | 'capacity.peakShortfall'
  | 'capacity.totalHours'
  | 'capacity.donut.title'
  | 'capacity.donut.subtitle'
  | 'capacity.donut.allocated'
  | 'capacity.donut.available'
  | 'capacity.donut.unavailable'
  | 'capacity.donut.centerLabel'
  | 'capacity.trend.title'
  | 'capacity.trend.subtitle'
  | 'capacity.hours.title'
  | 'capacity.hours.subtitle'
  | 'capacity.gaps.title'
  | 'capacity.gaps.subtitle'
  | 'capacity.gaps.noNeeds'
  | 'capacity.gaps.demand'
  | 'capacity.gaps.supply'
  | 'capacity.gaps.peakInWeek'
  | 'capacity.teams.title'
  | 'capacity.teams.subtitle'
  | 'capacity.skills.title'
  | 'capacity.skills.subtitle'
  | 'capacity.skills.empty'
  | 'capacity.bench.title'
  | 'capacity.bench.subtitle'
  | 'capacity.bench.empty'
  | 'capacity.bench.minAvailable'
  | 'capacity.understaffed.title'
  | 'capacity.understaffed.subtitle'
  | 'capacity.understaffed.empty'
  | 'capacity.understaffed.openRoles'
  | 'capacity.overloadWarning'
  | 'capacity.peopleCount'
  | 'capacity.noTeam'
  | 'guide.howToRead'
  | 'pages.dashboard.guide.step1'
  | 'pages.dashboard.guide.step2'
  | 'pages.dashboard.guide.step3'
  | 'pages.dashboard.hero.label'
  | 'pages.dashboard.hero.hint'
  | 'pages.dashboard.hero.healthy'
  | 'pages.dashboard.hero.attention'
  | 'pages.dashboard.quickAccessHint'
  | 'pages.skills.guide.step1'
  | 'pages.skills.guide.step2'
  | 'pages.skills.guide.step3'
  | 'pages.skills.hero.label'
  | 'pages.skills.hero.hint'
  | 'pages.skills.hero.categories'
  | 'pages.people.guide.step1'
  | 'pages.people.guide.step2'
  | 'pages.people.guide.step3'
  | 'pages.people.hero.label'
  | 'pages.people.hero.hint'
  | 'pages.people.hero.active'
  | 'pages.projects.guide.step1'
  | 'pages.projects.guide.step2'
  | 'pages.projects.guide.step3'
  | 'pages.projects.hero.label'
  | 'pages.projects.hero.hint'
  | 'pages.projects.hero.clients'
  | 'pages.projects.hero.inProgress'
  | 'pages.projects.team'
  | 'pages.projects.noTeam'
  | 'pages.projects.teamCount'
  | 'pages.needs.guide.step1'
  | 'pages.needs.guide.step2'
  | 'pages.needs.guide.step3'
  | 'pages.needs.hero.label'
  | 'pages.needs.hero.hint'
  | 'pages.needs.hero.open'
  | 'pages.needs.hero.partial'
  | 'pages.needs.kpi.total'
  | 'pages.needs.kpi.openDesc'
  | 'pages.needs.kpi.partialDesc'
  | 'pages.needs.kpi.totalDesc'
  | 'pages.conflicts.guide.step1'
  | 'pages.conflicts.guide.step2'
  | 'pages.conflicts.guide.step3'
  | 'pages.conflicts.hero.label'
  | 'pages.conflicts.hero.hint'
  | 'pages.conflicts.hero.healthy'
  | 'pages.conflicts.hero.weeks'
  | 'financials.totalRevenue'
  | 'financials.totalCost'
  | 'financials.totalMargin'
  | 'financials.marginAlerts'
  | 'alloc.prefillBanner'
  | 'pages.dashboard.title'
  | 'pages.dashboard.description'
  | 'pages.dashboard.people'
  | 'pages.dashboard.projects'
  | 'pages.dashboard.allocations'
  | 'pages.dashboard.conflicts'
  | 'pages.dashboard.understaffed'
  | 'pages.dashboard.marginAlerts'
  | 'pages.dashboard.layers'
  | 'pages.skills.title'
  | 'pages.skills.description'
  | 'pages.people.title'
  | 'pages.people.description'
  | 'pages.projects.title'
  | 'pages.projects.description'
  | 'pages.needs.title'
  | 'pages.needs.description'
  | 'pages.allocations.title'
  | 'pages.allocations.description'
  | 'pages.simulations.title'
  | 'pages.simulations.description'
  | 'pages.capacity.title'
  | 'pages.capacity.description'
  | 'pages.matching.title'
  | 'pages.matching.description'
  | 'pages.financials.title'
  | 'pages.financials.description'
  | 'pages.conflicts.title'
  | 'pages.conflicts.description'

const pt: Record<MessageKey, string> = {
  'app.name': 'IAS',
  'app.tagline': 'Gestão de alocação',
  'nav.dashboard': 'Painel',
  'nav.skills': 'Skills',
  'nav.people': 'Pessoas',
  'nav.projects': 'Projetos',
  'nav.needs': 'Necessidades',
  'nav.allocations': 'Alocações',
  'nav.simulations': 'Simulação',
  'nav.capacity': 'Capacidade',
  'nav.matching': 'Matching',
  'nav.financials': 'Financeiro',
  'nav.conflicts': 'Conflitos',
  'layout.tenant': 'Tenant (X-Tenant-Id)',
  'layout.actor': 'Ator (opcional)',
  'layout.tenantPlaceholder': '00000000-0000-0000-0000-000000000001',
  'layout.actorPlaceholder': 'GUID do usuário',
  'layout.tenantInvalid': 'Informe um GUID de tenant válido para carregar os dados.',
  'layout.themeLight': 'Claro',
  'layout.themeDark': 'Escuro',
  'layout.devConfig': 'Configuração de desenvolvimento',
  'layout.workspace': 'Área de trabalho',
  'layout.tenantReady': 'Tenant conectado',
  'layout.tenantPending': 'Tenant não configurado',
  'layout.api': 'API',
  'layout.apiChecking': 'Verificando…',
  'layout.apiDown': 'API indisponível. Inicie o backend em http://localhost:5203',
  'layout.hiddenOptions': 'Opções ocultas',
  'layout.menu': 'Menu',
  'layout.navMobile': 'Navegação principal',
  'layout.preferences': 'Preferências',
  'layout.language': 'Idioma',
  'nav.group.overview': 'Visão geral',
  'nav.group.team': 'Equipe e carteira',
  'nav.group.allocation': 'Alocação',
  'nav.group.planning': 'Planejamento',
  'nav.group.finance': 'Resultados',
  'common.loading': 'Carregando…',
  'common.save': 'Salvar',
  'common.cancel': 'Cancelar',
  'common.edit': 'Editar',
  'common.delete': 'Excluir',
  'common.select': 'Selecione…',
  'common.from': 'De',
  'common.to': 'Até',
  'common.yes': 'Sim',
  'common.no': 'Não',
  'common.add': 'Adicionar',
  'common.actions': 'Ações',
  'common.name': 'Nome',
  'common.category': 'Categoria',
  'common.none': '—',
  'table.skillSingular': 'competência',
  'table.skillPlural': 'competências',
  'common.confirmDelete': 'Excluir este registro?',
  'common.close': 'Fechar',
  'common.status': 'Status',
  'common.priority': 'Prioridade',
  'common.period': 'Período',
  'common.skills': 'Skills',
  'common.role': 'Papel',
  'common.details': 'Detalhes',
  'common.hide': 'Ocultar',
  'common.remove': 'Remover',
  'common.client': 'Cliente',
  'common.project': 'Projeto',
  'common.jobTitle': 'Cargo',
  'common.seniority': 'Senioridade',
  'common.weeklyHours': 'Horas/semana',
  'common.dedication': 'Dedicação %',
  'common.urgency': 'Urgência',
  'common.criticality': 'Criticidade',
  'common.startDate': 'Início',
  'common.endDate': 'Fim',
  'form.skills.new': 'Nova skill',
  'form.skills.edit': 'Editar skill',
  'form.people.new': 'Nova pessoa',
  'form.people.edit': 'Editar pessoa',
  'form.people.skillsOf': 'Skills de {name}',
  'form.people.noSkills': 'Nenhuma skill atribuída.',
  'form.people.assignSkill': 'Atribuir skill',
  'form.people.skillLevel': 'Nível',
  'form.projects.newClient': 'Novo cliente',
  'form.projects.new': 'Novo projeto',
  'form.projects.edit': 'Editar projeto',
  'form.projects.createClient': 'Criar cliente',
  'form.projects.create': 'Criar projeto',
  'form.projects.registeredClients': 'Clientes cadastrados',
  'form.projects.clientCount': '{count} cadastrado(s)',
  'form.projects.clientHasProjects': 'Este cliente possui projetos vinculados. Exclua ou mova os projetos antes.',
  'form.needs.editBanner': 'Editando necessidade',
  'form.needs.requiredSkills': 'Skills obrigatórias',
  'form.needs.skillsSelected': '{count} selecionada(s)',
  'form.needs.noRequiredSkills': 'Nenhuma skill obrigatória',
  'form.needs.create': 'Criar necessidade',
  'form.needs.saveChanges': 'Salvar alterações',
  'empty.skills': 'Nenhuma skill cadastrada.',
  'empty.people': 'Nenhuma pessoa cadastrada.',
  'empty.projects': 'Nenhum projeto cadastrado.',
  'empty.needs': 'Nenhuma necessidade cadastrada.',
  'empty.conflicts': 'Nenhum conflito detectado no tenant atual.',
  'empty.generic': 'Nenhum registro encontrado.',
  'capacity.weeksInPeriod': 'Semanas no período',
  'capacity.avgAllocated': 'Média alocada',
  'capacity.lastWeekHours': 'Horas alocadas (últ. semana)',
  'capacity.benchCount': 'Em bench (últ. semana)',
  'capacity.utilizationRate': 'Taxa de ocupação',
  'capacity.utilizationHint':
    'Percentual médio da capacidade da equipe já comprometida com projetos no período.',
  'capacity.howItWorks.title': 'Como ler esta página',
  'capacity.howItWorks.step1': 'Ocupação = quanto da equipe já está alocada em projetos.',
  'capacity.howItWorks.step2': 'Bench = pessoas com pelo menos 50% de capacidade livre.',
  'capacity.howItWorks.step3': 'Gaps = vagas abertas que ainda não foram preenchidas.',
  'capacity.avgAvailable': 'Disponível (média)',
  'capacity.openRoles': 'Projetos com vagas',
  'capacity.peakShortfall': 'Déficit máximo',
  'capacity.totalHours': 'Horas totais',
  'capacity.donut.title': 'Distribuição da capacidade',
  'capacity.donut.subtitle': 'Última semana do período — horas alocadas, livres e indisponíveis.',
  'capacity.donut.allocated': 'Alocado em projetos',
  'capacity.donut.available': 'Capacidade livre',
  'capacity.donut.unavailable': 'Indisponível (férias etc.)',
  'capacity.donut.centerLabel': 'horas totais',
  'capacity.trend.title': 'Evolução da ocupação',
  'capacity.trend.subtitle': 'Média semanal de % alocado vs % disponível da equipe.',
  'capacity.hours.title': 'Horas por semana',
  'capacity.hours.subtitle': 'Volume de horas: alocadas, livres e perdidas por indisponibilidade.',
  'capacity.gaps.title': 'Demanda vs oferta',
  'capacity.gaps.subtitle':
    'Vagas abertas (demanda) comparadas à capacidade livre agregada (oferta).',
  'capacity.gaps.noNeeds': 'Nenhuma necessidade aberta em projetos ativos.',
  'capacity.gaps.demand': 'Demanda (vagas)',
  'capacity.gaps.supply': 'Oferta (livre)',
  'capacity.gaps.peakInWeek': 'Pico na semana {week}',
  'capacity.teams.title': 'Ocupação por time',
  'capacity.teams.subtitle': 'Média de % alocado no período.',
  'capacity.skills.title': 'Ocupação por skill',
  'capacity.skills.subtitle': 'Skills com maior carga média no período.',
  'capacity.skills.empty': 'Nenhuma skill com pessoas no período.',
  'capacity.bench.title': 'Pessoas em bench',
  'capacity.bench.subtitle': '≥ 50% de capacidade livre em pelo menos uma semana.',
  'capacity.bench.empty': 'Ninguém em bench no período.',
  'capacity.bench.minAvailable': 'mín. livre',
  'capacity.understaffed.title': 'Projetos com vagas abertas',
  'capacity.understaffed.subtitle': 'Necessidades ainda não totalmente preenchidas.',
  'capacity.understaffed.empty': 'Todos os projetos com necessidades atendidas.',
  'capacity.understaffed.openRoles': '{count} vagas',
  'capacity.overloadWarning': '{count} pessoa(s) superalocada(s) na última semana',
  'capacity.peopleCount': 'pessoas',
  'capacity.noTeam': '(sem time)',
  'guide.howToRead': 'Como usar esta página',
  'pages.dashboard.guide.step1': 'O painel resume pessoas, projetos e alocações do tenant.',
  'pages.dashboard.guide.step2': 'Alertas em destaque: conflitos, vagas abertas e margem baixa.',
  'pages.dashboard.guide.step3': 'Use os atalhos à direita para ir direto ao módulo certo.',
  'pages.dashboard.hero.label': 'Pontos de atenção',
  'pages.dashboard.hero.hint': 'Soma de conflitos, projetos subalocados e alertas de margem.',
  'pages.dashboard.hero.healthy': 'Tudo certo',
  'pages.dashboard.hero.attention': 'Requer ação',
  'pages.dashboard.quickAccessHint': 'Acesso rápido às principais áreas do sistema.',
  'pages.skills.guide.step1': 'Cadastre competências usadas em pessoas, vagas e matching.',
  'pages.skills.guide.step2': 'Categoria é opcional — ajuda a agrupar skills parecidas.',
  'pages.skills.guide.step3': 'Skills são referência em todo o sistema; evite duplicar nomes.',
  'pages.skills.hero.label': 'Skills cadastradas',
  'pages.skills.hero.hint': 'Catálogo de competências técnicas do tenant.',
  'pages.skills.hero.categories': 'Com categoria',
  'pages.people.guide.step1': 'Cadastre a equipe com capacidade semanal em horas.',
  'pages.people.guide.step2': 'Status Active/Contractor entram no cálculo de capacidade.',
  'pages.people.guide.step3': 'Atribua skills para habilitar matching e simulações.',
  'pages.people.hero.label': 'Pessoas na equipe',
  'pages.people.hero.hint': 'Base para alocações, bench e score de candidatos.',
  'pages.people.hero.active': 'Ativas',
  'pages.projects.guide.step1': 'Projetos pertencem a um cliente e têm status de ciclo de vida.',
  'pages.projects.guide.step2': 'Projetos ativos alimentam necessidades e visão de capacidade.',
  'pages.projects.guide.step3': 'Crie clientes inline se ainda não existirem.',
  'pages.projects.hero.label': 'Projetos na carteira',
  'pages.projects.hero.hint': 'Onde as alocações e necessidades são vinculadas.',
  'pages.projects.hero.clients': 'Clientes',
  'pages.projects.hero.inProgress': 'Em andamento',
  'pages.projects.team': 'Equipe',
  'pages.projects.noTeam': 'Ninguém alocado neste projeto.',
  'pages.projects.teamCount': '{count} pessoa(s)',
  'pages.needs.guide.step1': 'Uma necessidade = uma vaga (papel + dedicação + período).',
  'pages.needs.guide.step2': 'Status Open/Partial alimentam capacidade, matching e gaps.',
  'pages.needs.guide.step3': 'Use o link de matching para ranquear candidatos.',
  'pages.needs.hero.label': 'Necessidades abertas',
  'pages.needs.hero.hint': 'Demanda de staffing que ainda precisa ser atendida.',
  'pages.needs.hero.open': 'Abertas',
  'pages.needs.hero.partial': 'Parciais',
  'pages.needs.kpi.total': 'Total ativas',
  'pages.needs.kpi.openDesc': 'Sem nenhuma alocação vinculada.',
  'pages.needs.kpi.partialDesc': 'Cobertura incompleta no período.',
  'pages.needs.kpi.totalDesc': 'Demanda de staffing ainda pendente.',
  'pages.conflicts.guide.step1': 'Conflito = pessoa com mais de 100% de dedicação na mesma semana.',
  'pages.conflicts.guide.step2': 'Revise as alocações listadas e ajuste dedicação ou datas.',
  'pages.conflicts.guide.step3': 'RN-001 bloqueia novas alocações que causem superalocação.',
  'pages.conflicts.hero.label': 'Conflitos ativos',
  'pages.conflicts.hero.hint': 'Semanas em que alguém ultrapassou 100% de dedicação.',
  'pages.conflicts.hero.healthy': 'Nenhum',
  'pages.conflicts.hero.weeks': 'Semanas afetadas',
  'financials.totalRevenue': 'Receita total',
  'financials.totalCost': 'Custo total',
  'financials.totalMargin': 'Margem total',
  'financials.marginAlerts': 'Alertas margem',
  'alloc.prefillBanner': 'Dados pré-preenchidos — revise e clique em Alocar.',
  'pages.dashboard.title': 'Painel',
  'pages.dashboard.description': 'Visão geral da consultoria: pessoas, projetos e indicadores de risco.',
  'pages.dashboard.people': 'Pessoas',
  'pages.dashboard.projects': 'Projetos',
  'pages.dashboard.allocations': 'Alocações',
  'pages.dashboard.conflicts': 'Conflitos',
  'pages.dashboard.understaffed': 'Subalocados',
  'pages.dashboard.marginAlerts': 'Alertas de margem',
  'pages.dashboard.layers': 'Módulos principais',
  'pages.skills.title': 'Skills',
  'pages.skills.description': 'Competências técnicas usadas em pessoas, necessidades e matching.',
  'pages.people.title': 'Pessoas',
  'pages.people.description': 'Equipe da consultoria: senioridade, custo e skills.',
  'pages.projects.title': 'Projetos',
  'pages.projects.description': 'Carteira de projetos e clientes.',
  'pages.needs.title': 'Necessidades de alocação',
  'pages.needs.description': 'Demandas de staffing por projeto. Alimenta matching e gaps.',
  'pages.allocations.title': 'Alocações',
  'pages.allocations.description': 'Vínculo pessoa ↔ projeto. Defina dedicação e período.',
  'pages.simulations.title': 'Simulação',
  'pages.simulations.description': 'Avalie viabilidade de staffing antes de vender um projeto.',
  'pages.capacity.title': 'Capacidade',
  'pages.capacity.description':
    'Quanto da equipe está ocupada, quem está livre e onde faltam pessoas.',
  'pages.matching.title': 'Matching',
  'pages.matching.description':
    'Ranqueie candidatos e aloque com um clique. A alocação é criada como Planejada.',
  'pages.financials.title': 'Financeiro',
  'pages.financials.description': 'Margem e custos por projeto.',
  'pages.conflicts.title': 'Conflitos',
  'pages.conflicts.description': 'Superalocações e sobreposições de agenda (RN-001).',
}

const en: Record<MessageKey, string> = {
  'app.name': 'IAS',
  'app.tagline': 'Allocation management',
  'nav.dashboard': 'Dashboard',
  'nav.skills': 'Skills',
  'nav.people': 'People',
  'nav.projects': 'Projects',
  'nav.needs': 'Needs',
  'nav.allocations': 'Allocations',
  'nav.simulations': 'Simulation',
  'nav.capacity': 'Capacity',
  'nav.matching': 'Matching',
  'nav.financials': 'Financials',
  'nav.conflicts': 'Conflicts',
  'layout.tenant': 'Tenant (X-Tenant-Id)',
  'layout.actor': 'Actor (optional)',
  'layout.tenantPlaceholder': '00000000-0000-0000-0000-000000000001',
  'layout.actorPlaceholder': 'User GUID',
  'layout.tenantInvalid': 'Enter a valid tenant GUID to load data.',
  'layout.themeLight': 'Light',
  'layout.themeDark': 'Dark',
  'layout.devConfig': 'Development settings',
  'layout.workspace': 'Workspace',
  'layout.tenantReady': 'Tenant connected',
  'layout.tenantPending': 'Tenant not configured',
  'layout.api': 'API',
  'layout.apiChecking': 'Checking…',
  'layout.apiDown': 'API unavailable. Start the backend at http://localhost:5203',
  'layout.hiddenOptions': 'Hidden options',
  'layout.menu': 'Menu',
  'layout.navMobile': 'Main navigation',
  'layout.preferences': 'Preferences',
  'layout.language': 'Language',
  'nav.group.overview': 'Overview',
  'nav.group.team': 'Team & portfolio',
  'nav.group.allocation': 'Allocation',
  'nav.group.planning': 'Planning',
  'nav.group.finance': 'Results',
  'common.loading': 'Loading…',
  'common.save': 'Save',
  'common.cancel': 'Cancel',
  'common.edit': 'Edit',
  'common.delete': 'Delete',
  'common.select': 'Select…',
  'common.from': 'From',
  'common.to': 'To',
  'common.yes': 'Yes',
  'common.no': 'No',
  'common.add': 'Add',
  'common.actions': 'Actions',
  'common.name': 'Name',
  'common.category': 'Category',
  'common.none': '—',
  'table.skillSingular': 'skill',
  'table.skillPlural': 'skills',
  'common.confirmDelete': 'Delete this record?',
  'common.close': 'Close',
  'common.status': 'Status',
  'common.priority': 'Priority',
  'common.period': 'Period',
  'common.skills': 'Skills',
  'common.role': 'Role',
  'common.details': 'Details',
  'common.hide': 'Hide',
  'common.remove': 'Remove',
  'common.client': 'Client',
  'common.project': 'Project',
  'common.jobTitle': 'Job title',
  'common.seniority': 'Seniority',
  'common.weeklyHours': 'Hours/week',
  'common.dedication': 'Dedication %',
  'common.urgency': 'Urgency',
  'common.criticality': 'Criticality',
  'common.startDate': 'Start',
  'common.endDate': 'End',
  'form.skills.new': 'New skill',
  'form.skills.edit': 'Edit skill',
  'form.people.new': 'New person',
  'form.people.edit': 'Edit person',
  'form.people.skillsOf': 'Skills for {name}',
  'form.people.noSkills': 'No skills assigned.',
  'form.people.assignSkill': 'Assign skill',
  'form.people.skillLevel': 'Level',
  'form.projects.newClient': 'New client',
  'form.projects.new': 'New project',
  'form.projects.edit': 'Edit project',
  'form.projects.createClient': 'Create client',
  'form.projects.create': 'Create project',
  'form.projects.registeredClients': 'Registered clients',
  'form.projects.clientCount': '{count} registered',
  'form.projects.clientHasProjects': 'This client has linked projects. Delete or reassign them first.',
  'form.needs.editBanner': 'Editing need',
  'form.needs.requiredSkills': 'Required skills',
  'form.needs.skillsSelected': '{count} selected',
  'form.needs.noRequiredSkills': 'No required skills',
  'form.needs.create': 'Create need',
  'form.needs.saveChanges': 'Save changes',
  'empty.skills': 'No skills registered.',
  'empty.people': 'No people registered.',
  'empty.projects': 'No projects registered.',
  'empty.needs': 'No needs registered.',
  'empty.conflicts': 'No conflicts detected for the current tenant.',
  'empty.generic': 'No records found.',
  'capacity.weeksInPeriod': 'Weeks in period',
  'capacity.avgAllocated': 'Avg. allocated',
  'capacity.lastWeekHours': 'Allocated hours (last week)',
  'capacity.benchCount': 'On bench (last week)',
  'capacity.utilizationRate': 'Utilization rate',
  'capacity.utilizationHint':
    'Average share of team capacity already committed to projects in the period.',
  'capacity.howItWorks.title': 'How to read this page',
  'capacity.howItWorks.step1': 'Utilization = how much of the team is allocated to projects.',
  'capacity.howItWorks.step2': 'Bench = people with at least 50% free capacity.',
  'capacity.howItWorks.step3': 'Gaps = open roles that are not fully staffed yet.',
  'capacity.avgAvailable': 'Available (avg.)',
  'capacity.openRoles': 'Projects with open roles',
  'capacity.peakShortfall': 'Peak shortfall',
  'capacity.totalHours': 'Total hours',
  'capacity.donut.title': 'Capacity distribution',
  'capacity.donut.subtitle': 'Last week in the period — allocated, free, and unavailable hours.',
  'capacity.donut.allocated': 'Allocated to projects',
  'capacity.donut.available': 'Free capacity',
  'capacity.donut.unavailable': 'Unavailable (time off, etc.)',
  'capacity.donut.centerLabel': 'total hours',
  'capacity.trend.title': 'Utilization trend',
  'capacity.trend.subtitle': 'Weekly average of allocated vs available team %.',
  'capacity.hours.title': 'Hours per week',
  'capacity.hours.subtitle': 'Hour volume: allocated, free, and lost to unavailability.',
  'capacity.gaps.title': 'Demand vs supply',
  'capacity.gaps.subtitle': 'Open roles (demand) compared to aggregate free capacity (supply).',
  'capacity.gaps.noNeeds': 'No open needs on active projects.',
  'capacity.gaps.demand': 'Demand (roles)',
  'capacity.gaps.supply': 'Supply (free)',
  'capacity.gaps.peakInWeek': 'Peak in week {week}',
  'capacity.teams.title': 'Utilization by team',
  'capacity.teams.subtitle': 'Average allocated % in the period.',
  'capacity.skills.title': 'Utilization by skill',
  'capacity.skills.subtitle': 'Skills with the highest average load in the period.',
  'capacity.skills.empty': 'No skills with people in the period.',
  'capacity.bench.title': 'People on bench',
  'capacity.bench.subtitle': '≥ 50% free capacity in at least one week.',
  'capacity.bench.empty': 'No one on bench in the period.',
  'capacity.bench.minAvailable': 'min. free',
  'capacity.understaffed.title': 'Projects with open roles',
  'capacity.understaffed.subtitle': 'Needs that are not fully filled yet.',
  'capacity.understaffed.empty': 'All projects have their needs met.',
  'capacity.understaffed.openRoles': '{count} roles',
  'capacity.overloadWarning': '{count} overallocated person(s) in the last week',
  'capacity.peopleCount': 'people',
  'capacity.noTeam': '(no team)',
  'guide.howToRead': 'How to use this page',
  'pages.dashboard.guide.step1': 'The dashboard summarizes people, projects, and allocations.',
  'pages.dashboard.guide.step2': 'Highlights: conflicts, open roles, and low margin alerts.',
  'pages.dashboard.guide.step3': 'Use shortcuts on the right to jump to the right module.',
  'pages.dashboard.hero.label': 'Attention items',
  'pages.dashboard.hero.hint': 'Sum of conflicts, understaffed projects, and margin alerts.',
  'pages.dashboard.hero.healthy': 'All clear',
  'pages.dashboard.hero.attention': 'Needs action',
  'pages.dashboard.quickAccessHint': 'Quick access to main areas of the system.',
  'pages.skills.guide.step1': 'Register skills used in people, needs, and matching.',
  'pages.skills.guide.step2': 'Category is optional — helps group similar skills.',
  'pages.skills.guide.step3': 'Skills are referenced everywhere; avoid duplicate names.',
  'pages.skills.hero.label': 'Registered skills',
  'pages.skills.hero.hint': 'Technical competency catalog for the tenant.',
  'pages.skills.hero.categories': 'With category',
  'pages.people.guide.step1': 'Register the team with weekly capacity in hours.',
  'pages.people.guide.step2': 'Active/Contractor status counts toward capacity.',
  'pages.people.guide.step3': 'Assign skills to enable matching and simulations.',
  'pages.people.hero.label': 'People on the team',
  'pages.people.hero.hint': 'Foundation for allocations, bench, and candidate scoring.',
  'pages.people.hero.active': 'Active',
  'pages.projects.guide.step1': 'Projects belong to a client and have a lifecycle status.',
  'pages.projects.guide.step2': 'Active projects feed needs and capacity views.',
  'pages.projects.guide.step3': 'Create clients inline if they do not exist yet.',
  'pages.projects.hero.label': 'Projects in portfolio',
  'pages.projects.hero.hint': 'Where allocations and needs are linked.',
  'pages.projects.hero.clients': 'Clients',
  'pages.projects.hero.inProgress': 'In progress',
  'pages.projects.team': 'Team',
  'pages.projects.noTeam': 'No one allocated to this project.',
  'pages.projects.teamCount': '{count} people',
  'pages.needs.guide.step1': 'A need = one open role (title + dedication + period).',
  'pages.needs.guide.step2': 'Open/Partial status feeds capacity, matching, and gaps.',
  'pages.needs.guide.step3': 'Use the matching link to rank candidates.',
  'pages.needs.hero.label': 'Open needs',
  'pages.needs.hero.hint': 'Staffing demand that still needs to be filled.',
  'pages.needs.hero.open': 'Open',
  'pages.needs.hero.partial': 'Partial',
  'pages.needs.kpi.total': 'Active total',
  'pages.needs.kpi.openDesc': 'No linked allocation yet.',
  'pages.needs.kpi.partialDesc': 'Incomplete coverage for the period.',
  'pages.needs.kpi.totalDesc': 'Staffing demand still pending.',
  'pages.conflicts.guide.step1': 'Conflict = person over 100% dedication in the same week.',
  'pages.conflicts.guide.step2': 'Review listed allocations and adjust dedication or dates.',
  'pages.conflicts.guide.step3': 'RN-001 blocks new allocations that cause overallocation.',
  'pages.conflicts.hero.label': 'Active conflicts',
  'pages.conflicts.hero.hint': 'Weeks where someone exceeded 100% dedication.',
  'pages.conflicts.hero.healthy': 'None',
  'pages.conflicts.hero.weeks': 'Affected weeks',
  'financials.totalRevenue': 'Total revenue',
  'financials.totalCost': 'Total cost',
  'financials.totalMargin': 'Total margin',
  'financials.marginAlerts': 'Margin alerts',
  'alloc.prefillBanner': 'Pre-filled data — review and click Allocate.',
  'pages.dashboard.title': 'Dashboard',
  'pages.dashboard.description':
    'Consultancy overview: people, projects, and risk indicators.',
  'pages.dashboard.people': 'People',
  'pages.dashboard.projects': 'Projects',
  'pages.dashboard.allocations': 'Allocations',
  'pages.dashboard.conflicts': 'Conflicts',
  'pages.dashboard.understaffed': 'Understaffed',
  'pages.dashboard.marginAlerts': 'Margin alerts',
  'pages.dashboard.layers': 'Main modules',
  'pages.skills.title': 'Skills',
  'pages.skills.description': 'Technical skills used in people, needs, and matching.',
  'pages.people.title': 'People',
  'pages.people.description': 'Consultancy team: seniority, cost, and skills.',
  'pages.projects.title': 'Projects',
  'pages.projects.description': 'Project portfolio and clients.',
  'pages.needs.title': 'Allocation needs',
  'pages.needs.description': 'Staffing demand per project. Feeds matching and gaps.',
  'pages.allocations.title': 'Allocations',
  'pages.allocations.description': 'Person ↔ project link. Set dedication and period.',
  'pages.simulations.title': 'Simulation',
  'pages.simulations.description': 'Assess staffing feasibility before selling a project.',
  'pages.capacity.title': 'Capacity',
  'pages.capacity.description':
    'How much of the team is busy, who is free, and where people are missing.',
  'pages.matching.title': 'Matching',
  'pages.matching.description':
    'Rank candidates and allocate in one click. Allocation is created as Planned.',
  'pages.financials.title': 'Financials',
  'pages.financials.description': 'Margin and costs per project.',
  'pages.conflicts.title': 'Conflicts',
  'pages.conflicts.description': 'Overallocation and schedule overlaps (RN-001).',
}

export const messages: Record<Locale, Record<MessageKey, string>> = { pt, en }

export function translate(locale: Locale, key: MessageKey, vars?: Record<string, string>): string {
  let text = messages[locale][key] ?? key
  if (vars) {
    for (const [k, v] of Object.entries(vars)) {
      text = text.replace(new RegExp(`\\{${k}\\}`, 'g'), v)
    }
  }
  return text
}
