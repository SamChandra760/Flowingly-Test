import { expect, test } from '@playwright/test'

test('submits text and displays parsed JSON and validation errors', async ({ page }) => {
  await page.goto('/')

  await page.getByRole('button', { name: 'Submit to server' }).click()

  await expect(page.getByText('"costCentre": "DEV632"')).toBeVisible()
  await expect(page.getByText('"salesTax": 4565.22')).toBeVisible()

  await page.getByLabel('Import source text').fill('<vendor>Seaside Steakhouse')
  await page.getByRole('button', { name: 'Submit to server' }).click()

  await expect(page.getByRole('alert')).toContainText('Opening tag <vendor> has no matching closing tag.')
})
