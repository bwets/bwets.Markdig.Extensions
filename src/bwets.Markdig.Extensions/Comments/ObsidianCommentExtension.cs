using Markdig;
using Markdig.Renderers;

namespace bwets.Markdig.Extensions.Comments;

/// <summary>
/// Markdig extension that hides Obsidian comments: inline <c>%%…%%</c> and <c>%%</c>-fenced blocks
/// are parsed and dropped so they never appear in the rendered output.
/// </summary>
public sealed class ObsidianCommentExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.BlockParsers.Contains<ObsidianCommentBlockParser>())
        {
            pipeline.BlockParsers.Insert(0, new ObsidianCommentBlockParser());
        }

        if (!pipeline.InlineParsers.Contains<ObsidianCommentInlineParser>())
        {
            pipeline.InlineParsers.Insert(0, new ObsidianCommentInlineParser());
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer html && !html.ObjectRenderers.Contains<ObsidianCommentRenderer>())
        {
            html.ObjectRenderers.Insert(0, new ObsidianCommentRenderer());
        }
    }
}
