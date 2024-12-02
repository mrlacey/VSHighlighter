using Newtonsoft.Json;

namespace VSHighlighter;

public class Highlight
{
	public Highlight()
	{
		this.Id = System.Guid.NewGuid().ToString();
	}

	[JsonIgnore]
	public string Id { get; private set; }
	public string FilePath { get; set; }
	public int LineNumber { get; set; } = -1;
	public int SpanStart { get; set; }
	public int SpanLength { get; set; }
	public HighlightColor Color { get; set; }
	public string Content { get; set; }

	public bool Intersects(int start, int length)
	{
		if (start < 0 || length < 0)
		{
			return false;
		}

		return (start <= this.SpanStart && start + length >= this.SpanStart)
			|| ((start >= this.SpanStart && start < this.SpanStart + this.SpanLength)
				|| (start + length >= this.SpanStart && start + length <= this.SpanStart + this.SpanLength));
	}

	public override bool Equals(object obj)
	{
		if (obj is Highlight other)
		{
			return
				FilePath == other.FilePath &&
				SpanStart == other.SpanStart &&
				SpanLength == other.SpanLength;
		}

		return false;
	}

	public override int GetHashCode()
	{
		return new { FilePath, SpanStart, SpanLength }.GetHashCode();
	}
}
