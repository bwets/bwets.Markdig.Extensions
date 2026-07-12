using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace bwets.Markdig.Extensions.Comments;

/// <summary>
/// Parses a multi-line Obsidian comment block: a line containing only <c>%%</c> opens it, and the
/// next such line closes it. Everything in between is swallowed and never rendered.
/// </summary>
public sealed class ObsidianCommentBlockParser : BlockParser
{
    public ObsidianCommentBlockParser()
    {
        OpeningCharacters = ['%'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent || !IsFence(processor.Line))
        {
            return BlockState.None;
        }

        processor.NewBlocks.Push(new ObsidianCommentBlock(this)
        {
            Column = processor.Column,
            Line = processor.LineIndex,
            Span = new SourceSpan(processor.Start, processor.Line.End),
        });

        return BlockState.ContinueDiscard; // opening "%%" consumed
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        // A closing "%%" ends the comment (and is consumed); any other line is swallowed.
        return IsFence(processor.Line) ? BlockState.BreakDiscard : BlockState.ContinueDiscard;
    }

    private static bool IsFence(StringSlice line) => line.ToString().Trim() == "%%";
}
