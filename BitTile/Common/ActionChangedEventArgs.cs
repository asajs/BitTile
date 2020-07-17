using System;

namespace BitTile.Common
{
	public class ActionChangedEventArgs : EventArgs
	{
		public ActionChangedEventArgs(string action)
		{
			Action = action;
		}

		public string Action { get; set; }
	}
}
