import type { ImportParseResponse, ValidationProblemDetails } from '../types/importTypes'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5150'

export async function parseImportText(text: string): Promise<ImportParseResponse> {
  const response = await fetch(`${apiBaseUrl}/api/imports/parse`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ text }),
  })

  if (response.ok) {
    return response.json()
  }

  if (response.status === 400) {
    const problem = (await response.json()) as ValidationProblemDetails
    throw new Error(getValidationMessages(problem).join('\n'))
  }

  throw new Error('The server returned an unexpected error.')
}

function getValidationMessages(problem: ValidationProblemDetails): string[] {
  const messages = Object.values(problem.errors ?? {}).flat()

  if (messages.length > 0) {
    return messages
  }

  return [problem.title ?? 'Unable to parse import text.']
}
