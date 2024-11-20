using Microsoft.VisualStudio.Text.Editor;

namespace VSHighlighter.Visuals;

internal class MarginTag : IGlyphTag
{
	public MarginTag(Highlight highlight) : this(highlight.Id, highlight.Color)
	{
	}

	public MarginTag(string id, HighlightColor color)
	{
		HighlightId = id;
		Color = color;
	}

	public HighlightColor Color { get; set; }

	public string HighlightId { get; set; }
}
