using Markdig;
using Markdig.Renderers;

namespace bwets.Markdig.Extensions.Tags;

/// <summary>Markdig extension that renders Obsidian inline tags (<c>#tag</c>) as styled spans.</summary>
public sealed class ObsidianTagExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<TagInlineParser>())
        {
            pipeline.InlineParsers.Insert(0, new TagInlineParser());
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer html && !html.ObjectRenderers.Contains<TagRenderer>())
        {
            html.ObjectRenderers.Insert(0, new TagRenderer());
        }
    }
}
