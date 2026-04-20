# Playwright E2E tests

This repo uses **Playwright for .NET** (NUnit) to run browser-based end-to-end tests against a running POS site.

## 1) Install Playwright browsers (one-time)

From `tests/POS.PlaywrightTests/`:

```powershell
dotnet build
powershell -ExecutionPolicy Bypass -File "bin\Debug\net8.0\playwright.ps1" install chromium
```

## 2) Run the app

From the repo root:

```powershell
dotnet run
```

By default the test expects the app at `https://localhost:5271`.

## 3) Run the tests

From `tests/POS.PlaywrightTests/`:

```powershell
$env:E2E_BASE_URL = "https://localhost:5271"
dotnet test
```


