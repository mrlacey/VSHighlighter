using Newtonsoft.Json;

namespace VSHighlighter;

internal class Highlight
{
	public Highlight()
	{
		this.Id = System.Guid.NewGuid().ToString();
	}

	[JsonIgnore]
	public string Id { get; private set; }
	public string FilePath { get; set; }
	public int SpanStart { get; set; }
	public int SpanLength { get; set; }
	public HighlightColor Color { get; set; }
	public string Content { get; set; }
}
