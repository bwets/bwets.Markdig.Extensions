using Markdig.Parsers;
using Markdig.Syntax;

namespace bwets.Markdig.Extensions.Admonitions;

/// <summary>
/// A parsed admonition / callout, produced from Docusaurus (<c>:::type[title] ... :::</c>),
/// MkDocs (<c>!!! type "title"</c> / <c>??? type "title"</c> + indented body), or
/// GitHub/Obsidian (<c>&gt; [!type] title</c> blockquote) syntax.
/// </summary>
public sealed class AdmonitionBlock : ContainerBlock
{
    public AdmonitionBlock(BlockParser parser) : base(parser)
    {
    }

    public string Kind { get; set; } = "note";

    /// <summary>null = default title (the kind), "" = no title, otherwise a custom title.</summary>
    public string? Title { get; set; }

    public bool Collapsible { get; set; }

    public bool Open { get; set; } = true;

    /// <summary>Number of opening fence characters (Docusaurus only).</summary>
    public int FenceLength { get; set; }
}
