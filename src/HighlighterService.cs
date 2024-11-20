using System.Collections.Generic;
using System.Threading.Tasks;

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
		var newHighlight = new Highlight
		{
			FilePath = filePath,
			SpanStart = start,
			SpanLength = length,
			Color = color,
			Content = string.Empty
		};

		System.Diagnostics.Debug.WriteLine($"Adding highlight {newHighlight.Id} to {newHighlight.FilePath}");

		highlights.Add(newHighlight);

		Messenger.RequestReloadHighlights();
	}

	internal async Task RemoveHighlightAsync(string id)
	{
		var itemRemoved = false;

		foreach (var item in highlights)
		{
			if (item.Id == id)
			{
				// TODO: need to track details of what's been removed so it can be passed via the messenger (Issue#1)
				System.Diagnostics.Debug.WriteLine($"Removing highlight {item.Id}");

				highlights.Remove(item);
				itemRemoved = true;
				break;
			}
		}

		if (!itemRemoved)
		{
			await OutputPane.Instance.WriteAsync($"Failed to remove highlight '{id}'. Collection contained {highlights.Count} items.");
		}

		Messenger.RequestReloadHighlights();
	}

	internal void RemoveAll(string filePath)
	{
		System.Diagnostics.Debug.WriteLine($"Removing all highlights from '{filePath}'");

		for (int i = highlights.Count - 1; i >= 0; i--)
		{
			Highlight item = highlights[i];
			if (item.FilePath == filePath)
			{
				System.Diagnostics.Debug.WriteLine($"Removing highlight {item.Id}");

				highlights.Remove(item);
			}
		}

		Messenger.RequestReloadHighlights();
	}
}
