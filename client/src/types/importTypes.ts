export type ImportParseResponse = {
  costCentre: string
  totalIncludingTax: number
  totalExcludingTax: number
  salesTax: number
  taxRate: number
  fields: Record<string, string>
}

export type ValidationProblemDetails = {
  title?: string
  status?: number
  errors?: Record<string, string[]>
}
