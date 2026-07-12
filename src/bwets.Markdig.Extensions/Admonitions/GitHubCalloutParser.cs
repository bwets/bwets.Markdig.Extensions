using Markdig.Parsers;
using Markdig.Syntax;

namespace bwets.Markdig.Extensions.Admonitions;

/// <summary>
/// Parses GitHub / Obsidian callouts written as a blockquote whose first line is a type marker —
/// <c>&gt; [!info] Optional title</c> — with the remaining <c>&gt;</c>-prefixed lines as the body.
/// An Obsidian fold marker makes the callout collapsible: <c>[!info]+</c> renders expanded,
/// <c>[!info]-</c> collapsed. When no title is given the kind is used (matching GitHub alerts).
/// </summary>
/// <remarks>
/// Registered ahead of Markdig's built-in quote parser: a leading line that is not a valid marker
/// returns <see cref="BlockState.None"/>, so ordinary blockquotes fall through unchanged.
/// </remarks>
public sealed class GitHubCalloutParser : BlockParser
{
    public GitHubCalloutParser()
    {
        OpeningCharacters = ['>'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        var line = processor.Line; // struct copy — look ahead without consuming.

        // Blockquote marker: '>' plus an optional single space.
        line.NextChar();
        if (line.CurrentChar == ' ')
        {
            line.NextChar();
        }

        // Require the "[!kind]" marker as the first content on the line.
        if (line.CurrentChar != '[')
        {
            return BlockState.None;
        }

        line.NextChar();
        if (line.CurrentChar != '!')
        {
            return BlockState.None;
        }

        line.NextChar();
        var kindStart = line.Start;
        while (line.CurrentChar != ']' && line.CurrentChar != '\0')
        {
            line.NextChar();
        }

        if (line.CurrentChar != ']')
        {
            return BlockState.None;
        }

        var kind = line.Text.Substring(kindStart, line.Start - kindStart).Trim();
        if (!AdmonitionInfo.IsWord(kind))
        {
            return BlockState.None;
        }

        // GitHub alerts are conventionally uppercase ([!NOTE]); normalise so the default title reads
        // "Note" rather than "NOTE" (kind → class lookup is case-insensitive regardless).
        kind = kind.ToLowerInvariant();

        line.NextChar(); // past ']'

        // Optional Obsidian fold marker: '+' expanded, '-' collapsed.
        var collapsible = false;
        var open = true;
        if (line.CurrentChar == '+')
        {
            collapsible = true;
            line.NextChar();
        }
        else if (line.CurrentChar == '-')
        {
            collapsible = true;
            open = false;
            line.NextChar();
        }

        // Remainder of the line is the (optional) custom title.
        line.TrimStart();
        var titleText = line.ToString().Trim();
        var title = titleText.Length > 0 ? titleText : null;

        processor.NewBlocks.Push(new AdmonitionBlock(this)
        {
            Column = processor.Column,
            Line = processor.LineIndex,
            Span = new SourceSpan(processor.Start, processor.Line.End),
            Kind = kind,
            Title = title,
            Collapsible = collapsible,
            Open = open,
        });

        // The marker line is fully consumed (title lives on the block); keep the block open so the
        // following '>' lines become its body.
        return BlockState.ContinueDiscard;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        // A line without a '>' marker (including a blank line) ends the callout, as with a blockquote.
        if (processor.IsCodeIndent || processor.CurrentChar != '>')
        {
            return BlockState.None;
        }

        // Strip the '>' and an optional single space, then parse the rest as body content.
        processor.NextChar();
        if (processor.CurrentChar == ' ' || processor.CurrentChar == '\t')
        {
            processor.NextColumn();
        }

        return BlockState.Continue;
    }
}
