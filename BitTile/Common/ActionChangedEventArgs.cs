using BitTile.Common.Interfaces;
using System;

namespace BitTile.Common
{
	public class ActionChangedEventArgs : EventArgs
	{
		public ActionChangedEventArgs(IAction action)
		{
			Action = action;
		}

		public IAction Action { get; set; }
	}
}
