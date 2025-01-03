using Microsoft.VisualStudio.Text.Tagging;

namespace VSHighlighter.Visuals;

public class VsHighlightTag : TextMarkerTag
{
	public VsHighlightTag(string type) : base(type)
	{
	}
}

