using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;

namespace VSHighlighter.Visuals;

[Export(typeof(IGlyphFactoryProvider))]
[Name("MarginGlyph")]
[Order(After = "VsTextMarker")]
[ContentType(StandardContentTypeNames.Any)]
[TagType(typeof(MarginTag))]
internal sealed class MarginGlyphFactoryProvider : IGlyphFactoryProvider
{
	public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
	{
		return new MarginGlyphFactory();
	}
}