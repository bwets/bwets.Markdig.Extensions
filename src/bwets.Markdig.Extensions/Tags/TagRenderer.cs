using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace bwets.Markdig.Extensions.Tags;

/// <summary>
/// Renders a <see cref="TagInline"/> as <c>&lt;span class="tag" data-tag="…"&gt;#…&lt;/span&gt;</c>
/// — a styleable, non-navigating pill. The <c>data-tag</c> holds the raw tag for optional hosting.
/// </summary>
public sealed class TagRenderer : HtmlObjectRenderer<TagInline>
{
    protected override void Write(HtmlRenderer renderer, TagInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("<span class=\"tag\" data-tag=\"");
            renderer.WriteEscape(obj.Tag);
            renderer.Write("\">#");
            renderer.WriteEscape(obj.Tag);
            renderer.Write("</span>");
        }
        else
        {
            renderer.Write('#').Write(obj.Tag);
        }
    }
}
