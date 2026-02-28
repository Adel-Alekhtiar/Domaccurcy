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
  ------------------------------CHATGPT PLAN
# AI-Based Website DOM Change Detection – Project Plan

## Project Goal

Build an intelligent system that:

- Loads websites
- Extracts and structures DOM nodes
- Stores snapshots
- Detects structural changes over time
- Learns how to recover broken selectors
- Eventually improves automatically using ML

---

# Phase 1 – Core Infrastructure (No ML Yet)

## 1. Website Loading Layer

**Technology (C#):**
- PuppeteerSharp or Playwright for .NET

### Responsibilities:

- Load full rendered pages
- Execute JavaScript
- Extract full HTML
- Capture timestamped snapshots
- **Extract all CSS used in the page**
- **Extract the entire CSS (inline, embedded, and external stylesheets)**
- Store CSS separately for structural and styling comparison

### CSS Extraction Details:

You must collect:

1. Inline styles (`style=""`)
2. `<style>` blocks
3. External stylesheets (`<link rel="stylesheet">`)
4. Computed styles (optional advanced phase)

### Output:

- Raw HTML per website
- Full CSS bundle
- Timestamped snapshot package

---

## 2. DOM Representation Layer

Convert HTML into structured tree objects.

### Example Model

```csharp
class DomNode
{
    public string TagName;
    public string Id;
    public List<string> Classes;
    public Dictionary<string, string> Attributes;
    public string InnerText;
    public List<DomNode> Children;
    public string XPath;
}
  - `Models/DomSnapshot` - Full page snapshot with HTML, CSS, and nodes
  - `Models/DomChange` - Represents a detected change
  - `Services/WebsiteLoader` - Loads pages using PuppeteerSharp
  - `Services/DomExtractor` - Parses HTML into DomNode tree
  - `Services/SnapshotStorage` - Saves/loads snapshots as JSON
  - `Services/ChangeDetector` - Compares snapshots and returns changes
  - `Services/StealthPlugin` - Provides stealth JS to avoid bot detection

- **Domaccurcy.App** - Console application entry point
