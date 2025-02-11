using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export(typeof(EditorFormatDefinition))]
[Name("MarkerFormatDefinition/LimeHighlightFormatDefinition")]
[UserVisible(false)]  // Don't want people changing this as then wouldn't match the margin and scrollbar
internal class LimeHighlightFormatDefinition : MarkerFormatDefinition
{
	public LimeHighlightFormatDefinition()
	{
		var background = Brushes.Lime.Clone();
		background.Opacity = SharedVisualSettings.HighlightOpacity;
		this.Fill = background;
		this.Border = new Pen(Brushes.Transparent, 1.0);
		this.DisplayName = "Lime Highlight";
		this.ZOrder = 5;
	}
}

