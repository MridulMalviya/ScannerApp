# ScannerApp

A cross-platform .NET MAUI app for scanning barcodes.

![Scanner demo](scanner.gif)

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

### iOS (Development IPA)

To generate an `.ipa` for **development** you must have Apple code-signing set up on your Mac:

- Apple Developer account
- iOS **Development** certificate installed in Keychain
- Development provisioning profile for your bundle id (default is `com.companyname.scannerapp`)

Verify signing identities:

```bash
security find-identity -v -p codesigning
```

Build the IPA:

```bash
dotnet publish -c Release -f net9.0-ios -p:RuntimeIdentifier=ios-arm64 -p:BuildIpa=true -p:ArchiveOnBuild=true
```

If you need to specify the signing key explicitly:

```bash
dotnet publish -c Release -f net9.0-ios -p:RuntimeIdentifier=ios-arm64 -p:BuildIpa=true -p:ArchiveOnBuild=true -p:CodesignKey="Apple Development: YOUR NAME (TEAMID)"
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
