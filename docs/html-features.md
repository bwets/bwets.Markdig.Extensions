# HTML features & asset delivery

The HTML feature model lets you compose features (each = Markdig config + its CSS/JS) into a page,
and choose how the assets reach the browser.

## The model

```csharp
public abstract class HtmlMarkdownFeature
{
    void Configure(MarkdownPipelineBuilder pipeline);   // optional: parsers/renderers
    IEnumerable<MarkdownAsset> Assets { get; }          // CSS/JS the feature needs
    string Head { get; }                                // extra <head> markup
    string BodyEnd { get; }                             // extra end-of-body markup (init)
}
```

`MarkdownHtmlFeatures` collects features and composes them.

## End-to-end

```csharp
using Markdig;
using bwets.Markdig.Extensions.Html;

var features = new MarkdownHtmlFeatures()
    .UseSyntaxHighlighting(extendedLanguages: true)
    .UseMermaid()
    .UseAdmonitions();

// 1. Build the Markdig pipeline
var builder = new MarkdownPipelineBuilder().UseAdvancedExtensions();
features.Configure(builder);
var body = Markdown.ToHtml(markdown, builder.Build());

// 2. Compose the page
var delivery = MarkdownAssetDelivery.External;          // or Inline
var html =
    "<!doctype html><html><head>" +
    features.RenderHead(delivery) +
    "</head><body>" + body +
    features.RenderBodyEnd(delivery) +
    "</body></html>";

// 3. For External delivery, write the asset files next to the page
if (delivery == MarkdownAssetDelivery.External)
{
    foreach (var asset in features.Assets)
        File.WriteAllText(Path.Combine(outputDir, asset.FileName), asset.Content);
}
```

## Delivery modes

| Mode | `RenderHead`/`RenderBodyEnd` emit | You must… |
|---|---|---|
| `Inline` | `<style>…</style>` / `<script>…</script>` | nothing — fully self-contained |
| `External` | `<link href>` / `<script src>` | write `features.Assets` files next to the page |

**Inline** works anywhere (a web response, an email-like sandbox, a host without a writable temp
dir). **External** keeps the HTML tiny and lets the browser cache big runtimes (highlight.js,
mermaid) — ideal for a desktop/WebView viewer.

## Ordering

Within each feature, style assets and `Head` go to the head; script assets then `BodyEnd` go to
the end of the body — so a runtime always loads before its initializer. Assets are de-duplicated
by file name across features.

## Writing your own feature

```csharp
public sealed class MyFeature : HtmlMarkdownFeature
{
    public override void Configure(MarkdownPipelineBuilder p) => p.Use<MyExtension>();
    public override string Head => "<style>.my { color: hotpink; }</style>";
}
```

Add an activation method and you're done:

```csharp
public static MarkdownHtmlFeatures UseMine(this MarkdownHtmlFeatures f) => f.Add(new MyFeature());
```
