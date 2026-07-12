using Markdig;
using bwets.Markdig.Extensions.Admonitions;
using bwets.Markdig.Extensions.Comments;
using bwets.Markdig.Extensions.Mermaid;
using bwets.Markdig.Extensions.Tags;
using bwets.Markdig.Extensions.Wiki;

namespace bwets.Markdig.Extensions;

/// <summary>
/// Extension methods to activate individual Markdig extensions on a <see cref="MarkdownPipelineBuilder"/>.
/// Each is independent, so only the ones you need have to be enabled.
/// </summary>
public static class MarkdownPipelineExtensions
{
    /// <summary>Enables Docusaurus (<c>:::</c>) and MkDocs (<c>!!!</c>/<c>???</c>) admonitions.</summary>
    public static MarkdownPipelineBuilder UseAdmonitions(this MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.Extensions.Contains<AdmonitionExtension>())
        {
            pipeline.Extensions.Add(new AdmonitionExtension());
        }

        return pipeline;
    }

    /// <summary>Renders <c>```mermaid</c> code blocks as mermaid diagram containers.</summary>
    public static MarkdownPipelineBuilder UseMermaid(this MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.Extensions.Contains<MermaidExtension>())
        {
            pipeline.Extensions.Add(new MermaidExtension());
        }

        return pipeline;
    }

    /// <summary>
    /// Enables Obsidian-style wiki links (<c>[[Target]]</c>, <c>[[Target|Display]]</c>,
    /// <c>[[Target#Heading]]</c>) and image embeds (<c>![[image.png]]</c>). They become standard
    /// links/images tagged with a <c>wikilink</c> CSS class, so the host resolves and navigates them
    /// like any other relative link.
    /// </summary>
    public static MarkdownPipelineBuilder UseWikiLinks(this MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.Extensions.Contains<WikiLinkExtension>())
        {
            pipeline.Extensions.Add(new WikiLinkExtension());
        }

        return pipeline;
    }

    /// <summary>
    /// Hides Obsidian comments: inline <c>%%…%%</c> and <c>%%</c>-fenced blocks are dropped from the
    /// rendered output.
    /// </summary>
    public static MarkdownPipelineBuilder UseObsidianComments(this MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.Extensions.Contains<ObsidianCommentExtension>())
        {
            pipeline.Extensions.Add(new ObsidianCommentExtension());
        }

        return pipeline;
    }

    /// <summary>
    /// Renders Obsidian inline tags (<c>#tag</c>, <c>#nested/tag</c>) as
    /// <c>&lt;span class="tag"&gt;</c> pills.
    /// </summary>
    public static MarkdownPipelineBuilder UseObsidianTags(this MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.Extensions.Contains<ObsidianTagExtension>())
        {
            pipeline.Extensions.Add(new ObsidianTagExtension());
        }

        return pipeline;
    }
}
