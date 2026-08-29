export function InfoTip({ text }: { text: string }) {
  return (
    <span className="ias-info-tip">
      <button type="button" className="ias-info-tip-trigger" aria-label={text}>
        <svg className="ias-info-tip-icon" viewBox="0 0 16 16" fill="none" aria-hidden>
          <circle cx="8" cy="8" r="6.25" stroke="currentColor" strokeWidth="1.25" />
          <path d="M8 7.1V11" stroke="currentColor" strokeWidth="1.35" strokeLinecap="round" />
          <circle cx="8" cy="5.15" r="0.85" fill="currentColor" />
        </svg>
      </button>
      <span role="tooltip" className="ias-info-tip-bubble">
        {text}
      </span>
    </span>
  )
}
