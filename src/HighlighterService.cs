using System.Collections.Generic;

namespace VSHighlighter
{
	internal class HighlighterService
	{

		private static HighlighterService instance;
		public static HighlighterService Instance => instance ?? (instance = new HighlighterService());

		public HighlighterService()
		{
			// TODO: Load any data from disk
		}

		internal IEnumerable<Highlight> GetHighlights(string fileName)
		{
			return new List<Highlight>
			{
				new Highlight
				{
					FileName = fileName,
					SpanStart = 105,
					SpanLength = 85,
					Color = HighlightColor.DarkTurquoise,
					Content = "TODO: Implement this"
				},
				new Highlight
				{
					FileName = fileName,
					SpanStart = 260,
					SpanLength = 55,
					Color = HighlightColor.Fuchsia,
					Content = "TODO: Implement this"
				},
			};
		}
	}

}
