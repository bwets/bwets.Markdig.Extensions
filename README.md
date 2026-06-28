# bwets.Markdig.Extensions

[![build](https://github.com/bwets/bwets.Markdig.Extensions/actions/workflows/build.yml/badge.svg)](https://github.com/bwets/bwets.Markdig.Extensions/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/bwets.Markdig.Extensions.svg)](https://www.nuget.org/packages/bwets.Markdig.Extensions/)

Modular [Markdig](https://github.com/xoofx/markdig) extensions for rendering rich Markdown to HTML — **admonitions**, **mermaid diagrams**, and **code syntax highlighting** — where each capability is independently activatable and ships its own self-contained CSS/JS assets.

Targets **.NET Standard 2.0**.

## Install

```bash
dotnet add package bwets.Markdig.Extensions
```

## Two ways to use it

### 1. Pipeline extensions (pure Markdig)

Activate the parsers/renderers on a `MarkdownPipelineBuilder`:

```csharp
using Markdig;
using bwets.Markdig.Extensions;

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseAdmonitions()   // Docusaurus ::: and MkDocs !!!/??? admonitions
    .UseMermaid()       // ```mermaid blocks -> <pre class="mermaid">
    .Build();

var html = Markdown.ToHtml(markdown, pipeline);
```

Note: these only affect the produced HTML markup. To actually *render* mermaid diagrams or
highlight code you also need the client-side assets — that's what the HTML feature model below
provides.

### 2. HTML features (config + assets in one place)

`MarkdownHtmlFeatures` bundles each feature's Markdig configuration **and** the CSS/JS it needs,
and composes them into a page. The host decides how assets are delivered.

```csharp
using Markdig;
using bwets.Markdig.Extensions.Html;

var features = new MarkdownHtmlFeatures()
    .UseSyntaxHighlighting(extendedLanguages: true)  // base ~38 vs extended ~190 + Razor
    .UseMermaid()
    .UseAdmonitions();

var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions();
features.Configure(pipeline);                  // wire up parsers/renderers
var html = Markdown.ToHtml(markdown, pipeline.Build());

// Compose the page. Choose how the assets are delivered:
var delivery = MarkdownAssetDelivery.Inline;   // or External
var page =
    "<!doctype html><html><head>" + features.RenderHead(delivery) + "</head><body>" +
    html + features.RenderBodyEnd(delivery) +
    "</body></html>";
```

#### Asset delivery

| Delivery | What it does | Good for |
|---|---|---|
| `Inline` | Embeds CSS/JS directly into the page (`<style>` / `<script>`) | Web pages, anywhere a temp file cache isn't available |
| `External` | Emits `<link>` / `<script src>`; you write `features.Assets` next to the page | Desktop/WebView hosts that want small HTML + cached files |

When delivering externally, write the files yourself:

```csharp
foreach (var asset in features.Assets)
    File.WriteAllText(Path.Combine(outputDir, asset.FileName), asset.Content);
```

## Features

| Feature | Activation | Notes |
|---|---|---|
| **Admonitions** | `UseAdmonitions()` | Docusaurus `:::type[title]…:::` and MkDocs `!!! type "title"` / collapsible `??? type` |
| **Mermaid** | `UseMermaid()` | `​```mermaid` → diagrams; bundles the all-in-one mermaid runtime |
| **Syntax highlighting** | `UseSyntaxHighlighting(extendedLanguages)` | highlight.js; choose the common ~38 languages or the full ~190 + Razor |

See [`docs/`](docs/index.md) for details on each feature.

## License

[MIT](LICENSE)
