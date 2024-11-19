using Microsoft.VisualStudio.Text.Editor;

namespace VSHighlighter.Visuals;

internal class MarginTag : IGlyphTag
{
	public HighlightColor Color { get; set; }
}
