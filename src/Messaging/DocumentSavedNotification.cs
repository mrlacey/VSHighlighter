namespace VSHighlighter.Messaging;

public class DocumentSavedNotification
{
	public DocumentSavedNotification(string fileName)
	{
		FileName = fileName;
	}

	public string FileName { get; set; }
}
