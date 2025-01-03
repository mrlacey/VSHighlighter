namespace VSHighlighter.Messaging;

public class RequestReloadHighlights
{
	public RequestReloadHighlights(string fileName)
	{
		FileName = fileName;
		this.WholeDocument = true;
	}

	public RequestReloadHighlights(string fileName, int rangeStart, int rangeLength)
	{
		WholeDocument = false;
		FileName = fileName;
		RangeStart = rangeStart;
		RangeLength = rangeLength;
	}

	public string FileName { get; set; }

	public bool WholeDocument { get; set; }

	public int RangeStart { get; set; } = -1;

	public int RangeLength { get; set; } = -1;
}
