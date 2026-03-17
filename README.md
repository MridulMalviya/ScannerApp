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

## License

MIT. See `LICENSE`.
