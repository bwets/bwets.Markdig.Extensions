using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace bwets.Markdig.Extensions.Wiki;

/// <summary>
/// Parses Obsidian-style wiki links — <c>[[Target]]</c>, <c>[[Target|Display]]</c>,
/// <c>[[Target#Heading]]</c> — and image embeds <c>![[image.png]]</c>. Each is emitted as a
/// standard Markdig <see cref="LinkInline"/> carrying a <c>wikilink</c> CSS class, so the host's
/// normal link/image pipeline (relative-path resolution, in-app navigation rewriting, image
/// inlining) applies to them unchanged.
/// </summary>
/// <remarks>
/// The parser opens on <c>[</c> and <c>!</c> but only matches the double-bracket form; anything
/// else is left for the built-in link/image parsers. It never spans a line break and rejects a
/// nested <c>[</c>, so an unterminated <c>[[</c> falls through to plain text.
/// </remarks>
public sealed class WikiLinkInlineParser : InlineParser
{
    public WikiLinkInlineParser()
    {
        OpeningCharacters = new[] { '[', '!' };
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var text = slice.Text;
        var open = slice.Start;
        var isImage = text[open] == '!';

        // Require the double bracket: "[[" for a link, "![[" for an embed.
        var bracket = isImage ? open + 1 : open;
        if (bracket + 1 > slice.End || text[bracket] != '[' || text[bracket + 1] != '[')
        {
            return false;
        }

        var contentStart = bracket + 2;

        // Find the closing "]]" on the same line. A newline or a stray '[' means this was not a
        // wiki link after all — bail so the default parsers get a chance.
        var close = -1;
        for (var i = contentStart; i < slice.End; i++)
        {
            var c = text[i];
            if (c == '\n' || c == '[')
            {
                return false;
            }

            if (c == ']' && text[i + 1] == ']')
            {
                close = i;
                break;
            }
        }

        if (close < 0)
        {
            return false;
        }

        var inner = text.Substring(contentStart, close - contentStart);
        if (!TrySplit(inner, out var target, out var display))
        {
            return false;
        }

        // Only "![[file.ext]]" pointing at an image becomes an <img>. A note embed like "![[Note]]"
        // can't be transcluded in a single-file viewer, so it degrades to a plain link to the note.
        var isImageEmbed = isImage && HasImageExtension(target);

        var startPos = processor.GetSourcePosition(open, out var line, out var column);

        var link = new LinkInline
        {
            Url = target,
            IsImage = isImageEmbed,
            IsClosed = true,
            Span = new SourceSpan(startPos, startPos + (close + 1 - open)),
            Line = line,
            Column = column,
        };
        link.GetAttributes().AddClass("wikilink");
        // Image alt = alias or file name; link/embedded-note label = alias or the readable target.
        link.AppendChild(new LiteralInline(display ?? (isImageEmbed ? target : DefaultLabel(target))));

        processor.Inline = link;
        slice.Start = close + 2;
        return true;
    }

    /// <summary>Splits the inner text on the first <c>|</c> into a required target and optional alias.</summary>
    private static bool TrySplit(string inner, out string target, out string? display)
    {
        target = string.Empty;
        display = null;

        // Inside a Markdown table the alias pipe must be escaped as "\|" to avoid being read as a
        // column separator (as Obsidian requires); normalise it back to a plain alias separator.
        inner = inner.Replace("\\|", "|");

        var pipe = inner.IndexOf('|');
        if (pipe >= 0)
        {
            target = inner.Substring(0, pipe).Trim();
            display = inner.Substring(pipe + 1).Trim();
            if (display.Length == 0)
            {
                display = null;
            }
        }
        else
        {
            target = inner.Trim();
        }

        return target.Length != 0;
    }

    /// <summary>Label shown when no alias is given: drops a leading <c>#</c> from a same-page link.</summary>
    private static string DefaultLabel(string target) =>
        target.StartsWith("#", StringComparison.Ordinal) ? target.Substring(1) : target;

    private static readonly string[] ImageExtensions =
        { ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp", ".bmp", ".ico", ".avif" };

    /// <summary>True when the embed target names an image file (so it should render as an <c>img</c>).</summary>
    private static bool HasImageExtension(string target)
    {
        foreach (var ext in ImageExtensions)
        {
            if (target.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
