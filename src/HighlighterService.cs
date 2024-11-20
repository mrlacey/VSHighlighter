using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

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
			Content = string.Empty
		});
		highlights.Add(new Highlight
		{
			FilePath = "C:\\Users\\matt\\source\\repos\\CsInlineColorTest\\MauiApp1\\MauiProgram.cs",
			SpanStart = 260,
			SpanLength = 55,
			Color = HighlightColor.Gold,
			Content = string.Empty
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

	internal void AddHighlight(string filePath, HighlightColor color, int start, int length)
	{
		highlights.Add(new Highlight
		{
			FilePath = filePath,
			SpanStart = start,
			SpanLength = length,
			Color = color,
			Content = string.Empty
		});

		Messenger.RequestReloadHighlights();
	}
}
