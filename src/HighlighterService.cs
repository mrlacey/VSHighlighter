using System.Collections.Generic;

namespace VSHighlighter;

internal class HighlighterService
{
	private List<Highlight> highlights = new();

	private static HighlighterService instance;
	public static HighlighterService Instance => instance ??= new HighlighterService();

	public HighlighterService()
	{
		// TODO: Load any data from disk
		highlights.Add(new Highlight
		{
			FileName = "Program.cs",
			SpanStart = 105,
			SpanLength = 85,
			Color = HighlightColor.DarkTurquoise,
			Content = "TODO: Implement this"
		});
		highlights.Add(new Highlight
		{
			FileName = "Program.cs",
			SpanStart = 260,
			SpanLength = 55,
			Color = HighlightColor.Fuchsia,
			Content = "TODO: Implement this"
		});
	}

	internal IEnumerable<Highlight> GetHighlights(string fileName)
	{
		foreach (var item in highlights)
		{
			if (item.FileName == fileName)
			{
				yield return item;
			}
		}
	}
}
