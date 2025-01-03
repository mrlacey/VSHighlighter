using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export(typeof(EditorFormatDefinition))]
[Name("MarkerFormatDefinition/GoldHighlightFormatDefinition")]
[UserVisible(false)]  // Don't want people changing this as then wouldn't match the margin and scrollbar
internal class GoldHighlightFormatDefinition : MarkerFormatDefinition
{
	public GoldHighlightFormatDefinition()
	{
		var background = Brushes.Gold.Clone();
		background.Opacity = 0.7;
		this.Fill = background;
		this.Border = new Pen(Brushes.Transparent, 1.0);
		this.DisplayName = "Gold Highlight";
		this.ZOrder = 5;
	}
}

