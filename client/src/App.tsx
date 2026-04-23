import { useMemo, useState } from 'react'
import './App.css'
import { parseImportText } from './api/importApi'
import type { ImportParseResponse } from './types/importTypes'

const sampleText = `Hi Patricia,

Please create an expense claim for the below. Relevant details are marked up as requested...
<expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal card</payment_method></expense>

From: William Steele
Sent: Friday, 16 June 2022 10:32 AM
To: Maria Washington
Subject: test

Hi Maria,

Please create a reservation for 10 at the <vendor>Seaside Steakhouse</vendor> for our <description>development team's project end celebration</description> on <date>27 April 2022</date> at 7.30pm.

Regards,
William`

function App() {
  const [text, setText] = useState(sampleText)
  const [result, setResult] = useState<ImportParseResponse | null>(null)
  const [errors, setErrors] = useState<string[]>([])
  const [isSubmitting, setIsSubmitting] = useState(false)

  const formattedResult = useMemo(() => {
    return result ? JSON.stringify(result, null, 2) : ''
  }, [result])

  async function handleSubmit() {
    setIsSubmitting(true)
    setResult(null)
    setErrors([])

    try {
      const parsedResult = await parseImportText(text)
      setResult(parsedResult)
    } catch (error) {
      setErrors(error instanceof Error ? error.message.split('\n') : ['Unable to parse import text.'])
    } finally {
      setIsSubmitting(false)
    }
  }

  function handleClear() {
    setText('')
    setResult(null)
    setErrors([])
  }

  return (
    <main className="app-shell">
      <header className="app-header">
        <h1>Extract tagged email data</h1>
      </header>

      <form
        className="workspace"
        onSubmit={(event) => {
          event.preventDefault()
          void handleSubmit()
        }}
      >
        <section className="panel" aria-labelledby="source-heading">
          <div className="panel-header">
            <h2 id="source-heading">Source text</h2>
          </div>

          <textarea
            value={text}
            onChange={(event) => setText(event.target.value)}
            aria-label="Import source text"
          />

          <div className="actions">
            <button type="submit" disabled={isSubmitting || text.trim().length === 0}>
              {isSubmitting ? 'Submitting...' : 'Submit to server'}
            </button>
            <button type="button" className="secondary" onClick={handleClear}>
              Clear input
            </button>
          </div>
        </section>

        <section className="panel" aria-labelledby="output-heading">
          <div className="panel-header">
            <h2 id="output-heading">Parsed output</h2>
            <span>{result ? 'Success' : errors.length > 0 ? 'Validation error' : 'Waiting'}</span>
          </div>

          {errors.length > 0 && (
            <div className="error-box" role="alert">
              <strong>Parsing failed</strong>
              <ul>
                {errors.map((error) => (
                  <li key={error}>{error}</li>
                ))}
              </ul>
            </div>
          )}

          {result ? (
            <pre>{formattedResult}</pre>
          ) : (
            <div className="empty-state">Submit source text to display the parsed JSON response.</div>
          )}
        </section>
      </form>
    </main>
  )
}

export default App
