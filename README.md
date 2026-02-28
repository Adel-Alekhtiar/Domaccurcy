# Domaccurcy

AI-Based Website DOM Change Detection tool built with .NET 10.

## Features

- Headless browser page loading via PuppeteerSharp
- Stealth mode JavaScript injection to avoid bot detection
- DOM extraction and tree building using HtmlAgilityPack
- Snapshot persistence as JSON files
- Change detection between snapshots (added, removed, modified nodes)

## Requirements

- .NET 10.0 SDK

## Usage

```bash
dotnet run --project src/Domaccurcy.App -- https://example.com
```

Or run without arguments to be prompted for a URL:

```bash
dotnet run --project src/Domaccurcy.App
```

## Architecture

- **Domaccurcy.Core** - Core library with models and services
  - `Models/DomNode` - Represents a DOM element
  - `Models/DomSnapshot` - Full page snapshot with HTML, CSS, and nodes
  - `Models/DomChange` - Represents a detected change
  - `Services/WebsiteLoader` - Loads pages using PuppeteerSharp
  - `Services/DomExtractor` - Parses HTML into DomNode tree
  - `Services/SnapshotStorage` - Saves/loads snapshots as JSON
  - `Services/ChangeDetector` - Compares snapshots and returns changes
  - `Services/StealthPlugin` - Provides stealth JS to avoid bot detection

- **Domaccurcy.App** - Console application entry point
