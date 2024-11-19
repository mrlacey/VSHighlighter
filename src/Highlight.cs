namespace VSHighlighter;

internal class Highlight
{
	public string FileName { get; set; }
	public int SpanStart { get; set; }
	public int SpanLength { get; set; }
	public HighlightColor Color { get; set; }
	public string Content { get; set; }
}
