# Admonitions

Callout blocks in both **Docusaurus** and **MkDocs** syntaxes.

## Activate

Pipeline only:

```csharp
new MarkdownPipelineBuilder().UseAdmonitions().Build();
```

With styling (HTML feature):

```csharp
new MarkdownHtmlFeatures().UseAdmonitions();
```

## Syntax

**Docusaurus** — fenced with `:::`, an optional `[title]`:

```text
:::tip
A helpful tip.
:::

:::warning[Read this first]
Custom title.
:::
```

**MkDocs** — `!!!` (always open) or `???` / `???+` (collapsible), an optional `"title"`, and an
indented body:

```text
!!! note "A note"
    Indented content belongs to the admonition.

??? danger "Click to expand"
    Hidden until opened.
```

## Output

```html
<div class="admonition admonition-tip">
  <div class="admonition-title"><span class="admonition-icon">💡</span>Tip</div>
  <div class="admonition-body"> … </div>
</div>
```

Collapsible admonitions render as `<details>` / `<summary>`. Type names are normalized
(`caution` → warning, `error`/`failure` → danger, `hint` → tip, …) to a small set of canonical
kinds: note, abstract, info, tip, success, question, warning, danger, bug, example, quote. A
title of `""` (MkDocs empty quotes) suppresses the title; omitting it uses the capitalized kind.

The `AdmonitionFeature` ships the matching CSS (per-kind accent colors, light/dark aware).
