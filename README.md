# Flowingly Full Stack Engineer Test Solution

This repository contains a solution for the **Full Stack Engineer Test (Senior) V2**.

## Stack

- Backend: `.NET 10` ASP.NET Core Web API
- Frontend: `React + TypeScript + Vite`
- Tests:
  - Backend unit tests with `xUnit`
  - Frontend E2E test with `Playwright`

## What The App Does

The API accepts a block of email/text content, extracts tagged values, and returns parsed JSON with tax calculations.

### Supported Input Pattern

- Embedded XML-like blocks (example: `<expense>...</expense>`)
- Inline tagged values (example: `<vendor>Seaside Steakhouse</vendor>`)

### Required Behaviors

- Reject message if opening tags do not have corresponding closing tags.
- Reject message if `<total>` is missing.
- If `<cost_centre>` is missing, default to `"UNKNOWN"`.
- Calculate:
  - `salesTax`
  - `totalExcludingTax`
  from `<total>` (which includes tax).

## Assumptions

The PDF does not provide a tax rate value, so this solution uses a configurable tax rate in:

- `src/FlowinglyImport.Api/appsettings.json`

```json
"Tax": {
  "Rate": 0.15
}
```

## Project Structure

```text
src/FlowinglyImport.Api
  Common/
  Controllers/
  Models/
  Options/
  Parsing/
  Services/

tests/FlowinglyImport.Api.Tests
  Parsing/
  Services/

client/
  src/
    api/
    types/
  e2e/
```

## Backend Run Instructions

From repo root:

```bash
dotnet restore FlowinglyImport.slnx
dotnet run --project src/FlowinglyImport.Api/FlowinglyImport.Api.csproj --launch-profile http
```

API base URL:

- `http://localhost:5150`

Swagger:

- `http://localhost:5150/swagger`

## Frontend Run Instructions

From `client/`:

```bash
npm install
npm run dev
```

Frontend URL:

- `http://localhost:5173`

The frontend calls:

- `http://localhost:5150/api/imports/parse`

You can override API URL with:

```bash
VITE_API_BASE_URL=http://localhost:5150
```

## Tests

### Backend Tests

From repo root:

```bash
dotnet test FlowinglyImport.slnx
```

### Frontend E2E Test

From `client/`:

```bash
npm run test:e2e
```

## Example API Request

`POST /api/imports/parse`

```json
{
  "text": "Please create expense <cost_centre>DEV632</cost_centre><total>35,000</total>"
}
```

## Notes

- Logging is added for parse requests and validation failures (without logging raw message content).
- CORS allows local frontend development origins:
  - `http://localhost:5173`
  - `http://127.0.0.1:5173`
