using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace bwets.Markdig.Extensions.Comments;

/// <summary>Renders an <see cref="ObsidianCommentBlock"/> as nothing (comments are hidden).</summary>
public sealed class ObsidianCommentRenderer : HtmlObjectRenderer<ObsidianCommentBlock>
{
    protected override void Write(HtmlRenderer renderer, ObsidianCommentBlock obj)
    {
        // Intentionally empty: the block and its content are omitted from the output.
    }
}
