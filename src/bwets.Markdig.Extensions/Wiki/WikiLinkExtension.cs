using Markdig;
using Markdig.Renderers;

namespace bwets.Markdig.Extensions.Wiki;

/// <summary>
/// Markdig extension that enables Obsidian-style wiki links and embeds via
/// <see cref="WikiLinkInlineParser"/>. It only registers the inline parser; the resulting
/// <c>LinkInline</c>s render through Markdig's built-in link/image renderer.
/// </summary>
public sealed class WikiLinkExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<WikiLinkInlineParser>())
        {
            // Insert ahead of the built-in link/image parsers so "[[" / "![[" win over "[" / "![".
            pipeline.InlineParsers.Insert(0, new WikiLinkInlineParser());
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }
}
