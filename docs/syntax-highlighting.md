# Code syntax highlighting

Highlights fenced code blocks with [highlight.js](https://highlightjs.org/).

Markdig already emits `language-*` classes on fenced code blocks, so there is **no pipeline
extension** for highlighting — it's purely a presentation feature.

## Activate

```csharp
new MarkdownHtmlFeatures().UseSyntaxHighlighting(extendedLanguages: false);
```

## Base vs extended languages

The `extendedLanguages` flag chooses the bundled highlight.js build:

| Value | Languages | Size | Use when |
|---|---|---|---|
| `false` (default) | common ~38 | ~125 KB | web pages, size matters |
| `true` | all ~190 + Razor | ~1.2 MB | a general viewer that must cover anything |

## What it ships

- the chosen highlight.js bundle (`highlight.min.js`)
- the GitHub light/dark themes (light by default, dark via `prefers-color-scheme`)
- an initializer (`hljs.highlightAll()`)

```html
<!-- head -->
<style>pre code.hljs { padding: 1rem; border-radius: 6px; } …themes… </style>
<!-- end of body -->
<script src="highlight.min.js"></script>
<script>hljs.highlightAll();</script>
```

When inlining assets, the feature framework escapes any `</script` inside the bundle so it can't
prematurely close the inline `<script>`.
