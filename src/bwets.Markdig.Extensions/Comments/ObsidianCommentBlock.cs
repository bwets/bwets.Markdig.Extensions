using Markdig.Parsers;
using Markdig.Syntax;

namespace bwets.Markdig.Extensions.Comments;

/// <summary>A <c>%%</c>-fenced Obsidian comment block. Its content is discarded and never rendered.</summary>
public sealed class ObsidianCommentBlock : Block
{
    public ObsidianCommentBlock(BlockParser parser) : base(parser)
    {
    }
}
