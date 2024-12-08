using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSHighlighter.Messaging;

namespace VSHighlighter;

internal class VsHighlighterRunningDocTableEvents : IVsRunningDocTableEvents
{
	private readonly AsyncPackage package;
	private readonly RunningDocumentTable runningDocumentTable;

	public VsHighlighterRunningDocTableEvents(AsyncPackage package, RunningDocumentTable runningDocumentTable)
	{
		this.package = package;
		this.runningDocumentTable = runningDocumentTable;
	}

	public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
	{
		return VSConstants.S_OK;
	}

	public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
	{
		return VSConstants.S_OK;
	}

	public int OnAfterSave(uint docCookie)
	{
		if (TryGetFilePath(docCookie, out var path))
		{
			WeakReferenceMessenger.Default.Send(new DocumentSavedNotification(path));
		}

		return VSConstants.S_OK;
	}

	public int OnAfterAttributeChange(uint docCookie, uint grfAttributes)
	{
		return VSConstants.S_OK;
	}

	private static string lastPath = string.Empty;

	public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
	{
		return VSConstants.S_OK;
	}

	public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
	{
		return VSConstants.S_OK;
	}

	private bool TryGetFilePath(uint docCookie, out string filePath)
	{
		try
		{
			var documentInfo = this.runningDocumentTable.GetDocumentInfo(docCookie);

			if (!string.IsNullOrWhiteSpace(documentInfo.Moniker))
			{
				filePath = documentInfo.Moniker;
				return true;
			}
		}
		catch (Exception exc)
		{
			System.Diagnostics.Debug.WriteLine(exc);
		}

		filePath = string.Empty;
		return false;
	}
}