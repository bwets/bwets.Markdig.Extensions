using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace bwets.Markdig.Extensions.Tags;

/// <summary>
/// Parses Obsidian inline tags: <c>#</c> followed by letters, digits, <c>_</c>, <c>-</c> or <c>/</c>,
/// where the body contains at least one non-digit character. Mirrors Obsidian's rules closely
/// enough to avoid false positives such as <c>C#</c> (preceded by a word character) and <c>#123</c>
/// (all digits).
/// </summary>
public sealed class TagInlineParser : InlineParser
{
    public TagInlineParser()
    {
        OpeningCharacters = ['#'];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var text = slice.Text;
        var start = slice.Start; // at '#'

        // The '#' must not be glued to a preceding word character (so "C#", "abc#def" are not tags).
        if (start > 0 && IsWordChar(text[start - 1]))
        {
            return false;
        }

        var i = start + 1;
        var hasQualifier = false; // a letter or '_' → distinguishes a tag from "#123"
        while (i <= slice.End && IsTagChar(text[i]))
        {
            var c = text[i];
            if (char.IsLetter(c) || c == '_')
            {
                hasQualifier = true;
            }

            i++;
        }

        if (i == start + 1 || !hasQualifier)
        {
            return false; // just "#", or an all-digit / punctuation-only body
        }

        var tag = text.Substring(start + 1, i - (start + 1));
        var startPos = processor.GetSourcePosition(start, out var line, out var column);

        processor.Inline = new TagInline
        {
            Tag = tag,
            Span = new SourceSpan(startPos, startPos + (i - 1 - start)),
            Line = line,
            Column = column,
        };
        slice.Start = i;
        return true;
    }

    private static bool IsTagChar(char c) =>
        char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '/';

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c);
}
