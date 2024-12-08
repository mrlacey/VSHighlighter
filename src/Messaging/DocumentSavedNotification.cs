﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSHighlighter.Messaging;

public class DocumentSavedNotification
{
	public DocumentSavedNotification(string fileName)
	{
		FileName = fileName;
	}

	public string FileName { get; set; }
}
