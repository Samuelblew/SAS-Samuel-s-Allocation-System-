import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { HiddenOptionsRoute } from './components/HiddenOptionsRoute'
import { Layout } from './components/Layout'
import { LocaleProvider } from './context/LocaleContext'
import { SettingsProvider } from './context/SettingsContext'
import { ThemeProvider } from './context/ThemeContext'
import { AllocationNeedsPage } from './pages/AllocationNeedsPage'
import { AllocationsPage } from './pages/AllocationsPage'
import { CapacityPage } from './pages/CapacityPage'
import { ConflictsPage } from './pages/ConflictsPage'
import { DashboardPage } from './pages/DashboardPage'
import { FinancialsPage } from './pages/FinancialsPage'
import { MatchingPage } from './pages/MatchingPage'
import { PeoplePage } from './pages/PeoplePage'
import { ProjectsPage } from './pages/ProjectsPage'
import { SimulationsPage } from './pages/SimulationsPage'
import { SkillsPage } from './pages/SkillsPage'

export default function App() {
  return (
    <ThemeProvider>
      <LocaleProvider>
        <SettingsProvider>
          <BrowserRouter>
            <Routes>
              <Route element={<Layout />}>
                <Route index element={<DashboardPage />} />
                <Route path="skills" element={<SkillsPage />} />
                <Route path="people" element={<PeoplePage />} />
                <Route path="projects" element={<ProjectsPage />} />
                <Route path="allocation-needs" element={<AllocationNeedsPage />} />
                <Route path="allocations" element={<AllocationsPage />} />
                <Route
                  path="simulations"
                  element={
                    <HiddenOptionsRoute path="/simulations">
                      <SimulationsPage />
                    </HiddenOptionsRoute>
                  }
                />
                <Route path="capacity" element={<CapacityPage />} />
                <Route path="matching" element={<MatchingPage />} />
                <Route
                  path="financials"
                  element={
                    <HiddenOptionsRoute path="/financials">
                      <FinancialsPage />
                    </HiddenOptionsRoute>
                  }
                />
                <Route path="conflicts" element={<ConflictsPage />} />
                <Route path="*" element={<Navigate to="/" replace />} />
              </Route>
            </Routes>
          </BrowserRouter>
        </SettingsProvider>
      </LocaleProvider>
    </ThemeProvider>
  )
}
