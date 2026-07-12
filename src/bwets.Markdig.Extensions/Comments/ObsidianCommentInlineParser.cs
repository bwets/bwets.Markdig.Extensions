using Markdig.Helpers;
using Markdig.Parsers;

namespace bwets.Markdig.Extensions.Comments;

/// <summary>
/// Parses an inline Obsidian comment — <c>%%hidden%%</c> — and discards it, emitting no output.
/// A lone or unterminated <c>%%</c> is left as literal text.
/// </summary>
public sealed class ObsidianCommentInlineParser : InlineParser
{
    public ObsidianCommentInlineParser()
    {
        OpeningCharacters = ['%'];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var text = slice.Text;
        var start = slice.Start;

        // Require an opening "%%".
        if (start + 1 > slice.End || text[start] != '%' || text[start + 1] != '%')
        {
            return false;
        }

        // Find the closing "%%".
        var close = -1;
        for (var i = start + 2; i < slice.End; i++)
        {
            if (text[i] == '%' && text[i + 1] == '%')
            {
                close = i;
                break;
            }
        }

        if (close < 0)
        {
            return false;
        }

        // Consume through the closing "%%" and emit nothing.
        slice.Start = close + 2;
        return true;
    }
}
