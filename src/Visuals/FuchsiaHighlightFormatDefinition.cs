using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export(typeof(EditorFormatDefinition))]
[Name("MarkerFormatDefinition/FuchsiaHighlightFormatDefinition")]
[UserVisible(false)]  // Don't want people changing this as then wouldn't match the margin and scrollbar
internal class FuchsiaHighlightFormatDefinition : MarkerFormatDefinition
{
	public FuchsiaHighlightFormatDefinition()
	{
		var background = Brushes.Fuchsia.Clone();
		background.Opacity = 0.7;
		this.Fill = background;
		this.Border = new Pen(Brushes.Transparent, 1.0);
		this.DisplayName = "Fuchsia Highlight";
		this.ZOrder = 5;
	}
}

