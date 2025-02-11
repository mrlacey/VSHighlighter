using System;
using System.Windows.Media;
using VSHighlighter.Visuals;

namespace VSHighlighter;

public static class HighlightColorsExtensions
{
	public static SolidColorBrush ToBrush(this HighlightColor color)
	{
		return color switch
		{
			HighlightColor.DarkTurquoise => new SolidColorBrush(Colors.DarkTurquoise) { Opacity = SharedVisualSettings.HighlightOpacity },
			HighlightColor.Fuchsia => new SolidColorBrush(Colors.Fuchsia) { Opacity = SharedVisualSettings.HighlightOpacity },
			HighlightColor.Gold => new SolidColorBrush(Colors.Gold) { Opacity = SharedVisualSettings.HighlightOpacity },
			HighlightColor.Lime => new SolidColorBrush(Colors.Lime) { Opacity = SharedVisualSettings.HighlightOpacity },
			_ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
		};
	}
}
