namespace VSHighlighter;

public static class Messenger
{
	// TODO: have the reload request be filtered to the span of the specific file changed (Issue#1)
	public delegate void ReloadHighlightsEventHandler();

	public static event ReloadHighlightsEventHandler ReloadHighlights;

	public static void RequestReloadHighlights()
	{
		System.Diagnostics.Debug.WriteLine("RequestReloadHighlights");
		ReloadHighlights?.Invoke();
	}
}
