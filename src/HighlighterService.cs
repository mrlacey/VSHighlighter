using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;

namespace VSHighlighter;

internal class HighlighterService
{
	private List<Highlight> highlights = new();

	private static HighlighterService instance;
	public static HighlighterService Instance => instance ??= new HighlighterService();

	private string filePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "vshighlighter.data");

	public HighlighterService()
	{
		if (File.Exists(filePath))
		{
			try
			{
				string jsonString = File.ReadAllText(filePath);
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
			File.WriteAllText(filePath, jsonString);
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

	internal async Task AddHighlightAsync(string filePath, HighlightColor color, int start, int length)
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

		WeakReferenceMessenger.Default.Send<RequestReloadHighlights>(new RequestReloadHighlights(start, length));

		await SaveAsync();
	}

	internal async Task RemoveHighlightAsync(string id)
	{
		var itemRemoved = false;

		int removalStart = -1;
		int removalLength = -1;

		foreach (var item in highlights)
		{
			if (item.Id == id)
			{
				System.Diagnostics.Debug.WriteLine($"Removing highlight {item.Id}");

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
			WeakReferenceMessenger.Default.Send(new RequestReloadHighlights(removalStart, removalLength));

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

		WeakReferenceMessenger.Default.Send(new RequestReloadHighlights());

		await SaveAsync();
	}
}
