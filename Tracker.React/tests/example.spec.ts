import { test, expect } from '@playwright/test';

test('login page', async ({ page }) => {
  await page.goto('http://localhost:8080/login');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle('Task tracker');
  await page.getByRole('button', { name: 'Войти' }).click();
  await expect(page.locator('[data-testid="typography"]', { hasText: 'Неправильный email или пароль' })).not.toBeNull();
});

test('login', async ({ page }) => {
  await page.goto('http://localhost:8080/login');
  await page.locator("[name='email']").fill('admin@parma.ru');
  await page.locator("[name='password']").fill('1');
  await page.getByRole('button', { name: 'Войти' }).click();

  await page.waitForURL('http://localhost:8080/');
  await expect(page.getByRole('menu')).toBeVisible();
});

test('generated login', async ({ page }) => {
  await page.goto('http://localhost:8080/');
  await page.goto('http://localhost:8080/login');
  await page.getByLabel('Email').click();
  await page.getByLabel('Email').fill('admin@parma.ru');
  await page.getByLabel('Пароль').click();
  await page.getByLabel('Пароль').fill('1');
  await page.getByRole('button', { name: 'Войти' }).click();

  await page.waitForURL('http://localhost:8080/');
  await expect(page.getByRole('menu')).toBeVisible();
});
