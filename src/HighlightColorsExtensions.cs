using System;

namespace VSHighlighter
{
	public static class HighlightColorsExtensions
	{
		public static SolidColorBrush ToBrush(this HighlightColor color)
		{
			switch (color)
			{
				case HighlightColor.DarkTurquoise:
					return new SolidColorBrush(Colors.DarkTurquoise);
				case HighlightColor.Fuchsia:
					return new SolidColorBrush(Colors.Fuchsia);
				case HighlightColor.Gold:
					return new SolidColorBrush(Colors.Gold);
				case HighlightColor.Lime:
					return new SolidColorBrush(Colors.Lime);
				default:
					throw new ArgumentOutOfRangeException(nameof(color), color, null);
			}
		}
	}

}
