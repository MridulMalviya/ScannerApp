# ScannerApp

A cross-platform .NET MAUI app for scanning barcodes.

## Features

- Barcode scanning via `BarcodeScanning.Native.Maui`
- Targets Android, iOS, Mac Catalyst (and Windows when building on Windows)

## Prerequisites

- .NET SDK (repo targets .NET 9 / MAUI)
- MAUI workloads installed

Install workloads:

```bash
dotnet workload install maui
```

## Run

Restore:

```bash
dotnet restore
```

### Android

```bash
dotnet build -t:Run -f net9.0-android
```

### iOS (macOS only)

```bash
dotnet build -t:Run -f net9.0-ios
```

### Mac Catalyst (macOS only)

```bash
dotnet build -t:Run -f net9.0-maccatalyst
```

### Windows (Windows only)

```bash
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

## Notes

- Camera permissions are required for scanning (handled per platform by MAUI).

## Automation (BrowserStack camera injection)

If you run automated tests on BrowserStack and need to test scanning flows, use BrowserStack’s **Camera Image Injection** feature.

- Docs: [Camera image injection (App Automate)](https://www.browserstack.com/docs/app-automate/appium/advanced-features/camera-image-injection)
- BrowserStack home: https://www.browserstack.com/

Important: **`cameraImageInjectionUrl` must be a direct link to an image** (`.png` / `.jpg`), not a normal webpage URL (so `https://www.browserstack.com/` by itself won’t work as the injection payload).

Example Appium capabilities (conceptual):

```json
{
  "enableCameraImageInjection": true,
  "cameraImageInjectionUrl": "https://your-hosted-assets.example.com/test-barcode.png"
}
```

## License

MIT. See `LICENSE`.
