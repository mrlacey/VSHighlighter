using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace VSHighlighter.Visuals;

[Export(typeof(ITaggerProvider))]
[ContentType(StandardContentTypeNames.Text)]
[TagType(typeof(MarginTag))]
class MarginTaggerProvider : ITaggerProvider
{
	[Import]
	internal IClassifierAggregatorService AggregatorService;

		[Import]
	internal ITextDocumentFactoryService docFactory = null;

	public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}

		return new MarginTagger(docFactory) as ITagger<T>;
	}
}
