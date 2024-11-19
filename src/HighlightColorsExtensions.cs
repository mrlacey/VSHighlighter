using System;
using System.Windows.Media;

namespace VSHighlighter;

public static class HighlightColorsExtensions
{
	public static SolidColorBrush ToBrush(this HighlightColor color)
	{
		return color switch
		{
			HighlightColor.DarkTurquoise => new SolidColorBrush(Colors.DarkTurquoise),
			HighlightColor.Fuchsia => new SolidColorBrush(Colors.Fuchsia),
			HighlightColor.Gold => new SolidColorBrush(Colors.Gold),
			HighlightColor.Lime => new SolidColorBrush(Colors.Lime),
			_ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
		};
	}
}
