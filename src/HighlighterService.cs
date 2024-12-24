using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.Text;
using Newtonsoft.Json;

namespace VSHighlighter;

internal class HighlighterService
{
	private List<Highlight> highlights = [];

	private static HighlighterService instance;
	public static HighlighterService Instance => instance ??= new HighlighterService();

	private readonly string persistedFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "vshighlighter.data");

	public HighlighterService()
	{
		if (File.Exists(persistedFilePath))
		{
			try
			{
				string jsonString = File.ReadAllText(persistedFilePath);
				highlights = JsonConvert.DeserializeObject<List<Highlight>>(jsonString);
			}
			catch (Exception ex)
			{
				_ = OutputPane.Instance.WriteAsync($"Error loading highlights: {ex.Message}");
				_ = OutputPane.Instance.WriteAsync(ex.StackTrace);
				_ = OutputPane.Instance.ActivateAsync();
			}
		}
	}

	private async Task SaveAsync()
	{
		try
		{
			string jsonString = JsonConvert.SerializeObject(highlights);
			File.WriteAllText(persistedFilePath, jsonString);
		}
		catch (Exception exc)
		{
			await OutputPane.Instance.WriteAsync($"Error saving highlights: {exc.Message}");
			await OutputPane.Instance.WriteAsync(exc.StackTrace);
			await OutputPane.Instance.ActivateAsync();
		}
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

	internal (int Start, int Length) GetHighlightSpan(string fileName, string id)
	{
		var highlightOfInterest = highlights.FirstOrDefault(x => x.FilePath == fileName && x.Id == id);

		if (highlightOfInterest != null)
		{
			return (highlightOfInterest.SpanStart, highlightOfInterest.SpanLength);
		}

		return (-1, -1);
	}

	internal async Task AddHighlightAsync(string filePath, HighlightColor color, int lineNo, int start, int length)
	{
		var newHighlight = new Highlight
		{
			FilePath = filePath,
			LineNumber = lineNo,
			SpanStart = start,
			SpanLength = length,
			Color = color,
			Content = string.Empty
		};

		System.Diagnostics.Debug.WriteLine($"Adding highlight {newHighlight.Id} to {newHighlight.FilePath}");

		if (highlights.Contains(newHighlight))
		{
			await OutputPane.Instance.WriteAsync($"Highlight already exists at {newHighlight.FilePath}:{newHighlight.SpanStart}-{newHighlight.SpanLength}");
			return;
		}

		highlights.Add(newHighlight);

		WeakReferenceMessenger.Default.Send(new RequestReloadHighlights(filePath, start, length));

		await SaveAsync();
	}

	internal async Task RemoveHighlightAsync(string id)
	{
		var itemRemoved = false;

		int removalStart = -1;
		int removalLength = -1;
		string removalFilePath = string.Empty;

		foreach (var item in highlights)
		{
			if (item.Id == id)
			{
				System.Diagnostics.Debug.WriteLine($"Removing highlight {item.Id}");

				removalFilePath = item.FilePath;
				removalStart = item.SpanStart;
				removalLength = item.SpanLength;

				highlights.Remove(item);
				itemRemoved = true;
				break;
			}
		}

		if (!itemRemoved)
		{
			await OutputPane.Instance.WriteAsync($"Failed to remove highlight '{id}'. Collection contained {highlights.Count} items.");
		}
		else
		{
			WeakReferenceMessenger.Default.Send(new RequestReloadHighlights(removalFilePath, removalStart, removalLength));

			await SaveAsync();
		}
	}

	internal async Task RemoveHighlightsInRangeAsync(string filePath, int start, int length)
	{
		var itemRemoved = false;

		int removalStart = -1;
		int removalLength = -1;

		for (int i = highlights.Count - 1; i >= 0; i--)
		{
			Highlight item = highlights[i];

			if (item.FilePath == filePath & item.Intersects(start, length))
			{
				System.Diagnostics.Debug.WriteLine($"Removing highlight {item.Id}");

				if (item.SpanStart < removalStart || removalStart == -1)
				{
					if (removalStart == -1)
					{
						removalLength = item.SpanLength;
					}
					else
					{
						removalLength += removalStart - item.SpanStart;
					}

					removalStart = item.SpanStart;
				}
				else if (removalStart != 1)
				{
					removalLength = removalStart + (item.SpanStart - removalStart) + item.SpanLength;
				}

				highlights.Remove(item);
				itemRemoved = true;
			}
		}

		if (!itemRemoved)
		{
			await OutputPane.Instance.WriteAsync($"Failed to remove highlights from '{filePath}' in range '{start}>{length}'. Collection contained {highlights.Count} items.");
		}
		else
		{
			WeakReferenceMessenger.Default.Send(new RequestReloadHighlights(filePath, removalStart, removalLength));

			await SaveAsync();
		}
	}

	internal async Task RemoveAllAsync(string filePath)
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

		WeakReferenceMessenger.Default.Send(new RequestReloadHighlights(filePath));

		await SaveAsync();
	}

	internal async Task UpdateHighlightAsync(string fileName, string id, int lineNo, int start, int length)
	{
		var highlightOfInterest = highlights.FirstOrDefault(x => x.FilePath == fileName && x.Id == id);

		if (highlightOfInterest == null)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to find highlight to update '{fileName}' : {id}");

			return;
		}

		highlightOfInterest.LineNumber = lineNo;
		highlightOfInterest.SpanStart = start;
		highlightOfInterest.SpanLength = length;

		await SaveAsync();
	}
}
