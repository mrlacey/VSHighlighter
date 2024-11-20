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
			FilePath = "C:\\Users\\matt\\source\\repos\\CsInlineColorTest\\MauiApp1\\MauiProgram.cs",
			SpanStart = 105,
			SpanLength = 85,
			Color = HighlightColor.Lime,
			Content = "TODO: Implement this"
		});
		highlights.Add(new Highlight
		{
			FilePath = "C:\\Users\\matt\\source\\repos\\CsInlineColorTest\\MauiApp1\\MauiProgram.cs",
			SpanStart = 260,
			SpanLength = 55,
			Color = HighlightColor.Gold,
			Content = "TODO: Implement this"
		});
	}

	internal IEnumerable<Highlight> GetHighlights(string fileName)
	{
		foreach (var item in highlights)
		{
			if (item.FilePath == fileName)
			{
				yield return item;
			}
		}
	}
}
