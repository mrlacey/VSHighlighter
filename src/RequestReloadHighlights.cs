namespace VSHighlighter;

public class RequestReloadHighlights
{
	public RequestReloadHighlights()
	{
		this.WholeDocument = true;
	}

	public RequestReloadHighlights(int rangeStart, int rangeLength)
	{
		WholeDocument = false;
		RangeStart = rangeStart;
		RangeLength = rangeLength;
	}

	public bool WholeDocument { get; set; }

	public int RangeStart { get; set; } = -1;

	public int RangeLength { get; set; } = -1;
}
