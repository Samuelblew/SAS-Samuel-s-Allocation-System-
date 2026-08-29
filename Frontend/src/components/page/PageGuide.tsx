import { Card } from '../Card'

export function PageGuide({ title, steps }: { title: string; steps: string[] }) {
  return (
    <Card className="ias-page-guide mb-4">
      <p className="ias-page-guide__title">{title}</p>
      <ol className="ias-page-guide__steps">
        {steps.map((step) => (
          <li key={step}>{step}</li>
        ))}
      </ol>
    </Card>
  )
}
