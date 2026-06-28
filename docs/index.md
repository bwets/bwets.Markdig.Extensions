# bwets.Markdig.Extensions

Modular [Markdig](https://github.com/xoofx/markdig) extensions for rendering rich Markdown to
HTML. Each capability is independent and ships its own self-contained CSS/JS, so you enable only
what you need and the host decides how assets are delivered.

- [Admonitions](admonitions.md) — Docusaurus & MkDocs callouts
- [Mermaid](mermaid.md) — diagrams from `​```mermaid` blocks
- [Syntax highlighting](syntax-highlighting.md) — highlight.js, base or extended languages
- [HTML features & asset delivery](html-features.md) — the composition model

## Installation

```bash
dotnet add package bwets.Markdig.Extensions
```

Targets **.NET Standard 2.0**; the only dependency is Markdig.

## Two layers

**1 — Pipeline extensions** affect the produced HTML markup only:

```csharp
using bwets.Markdig.Extensions;

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseAdmonitions()
    .UseMermaid()
    .Build();
```

**2 — HTML features** add the client-side assets (CSS/JS) needed to actually *present* the
markup, and compose them into a page:

```csharp
using bwets.Markdig.Extensions.Html;

var features = new MarkdownHtmlFeatures()
    .UseSyntaxHighlighting(extendedLanguages: true)
    .UseMermaid()
    .UseAdmonitions();
```

See [HTML features & asset delivery](html-features.md) for the full composition flow.

## Versioning

Releases are `0.1.<build>` where the build number increases on every CI build; pushes to `main`
publish automatically to NuGet.
