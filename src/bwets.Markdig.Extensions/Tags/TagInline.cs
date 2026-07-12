using Markdig.Syntax.Inlines;

namespace bwets.Markdig.Extensions.Tags;

/// <summary>An Obsidian inline tag such as <c>#todo/study</c>.</summary>
public sealed class TagInline : Inline
{
    /// <summary>The tag text without the leading <c>#</c> (e.g. <c>todo/study</c>).</summary>
    public string Tag { get; set; } = string.Empty;
}
