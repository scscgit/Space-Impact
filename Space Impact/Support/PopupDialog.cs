using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Support
{
	public static class PopupDialog
	{
		private const string TAG = "PopupDialog";

		public delegate void PopupDialogActionDelegate();
		public async static Task ShowPopupDialog(string question, string yes, PopupDialogActionDelegate actionYes, string no, PopupDialogActionDelegate actionNo, bool defaultYes, string third, PopupDialogActionDelegate actionThird)
		{
			var dialog = new Windows.UI.Popups.MessageDialog(question == null ? "<NO TEXT AVAILABLE>" : question);

			dialog.Commands.Add(new Windows.UI.Popups.UICommand(yes == null ? "Yes" : yes) { Id = 0 });
			dialog.Commands.Add(new Windows.UI.Popups.UICommand(no == null ? "No" : no) { Id = 1 });

			if (third != null && actionThird != null && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
			{
				//Adding a 3rd command will crash the app when running on Mobile !!!
				dialog.Commands.Add(new Windows.UI.Popups.UICommand(third) { Id = 2 });
			}

			if (defaultYes)
			{
				dialog.DefaultCommandIndex = 0;
			}
			else
			{
				dialog.DefaultCommandIndex = 1;
			}
			dialog.CancelCommandIndex = 1;

			switch ((int)(await dialog.ShowAsync()).Id)
			{
				case 0:
					Log.i(TAG, "User has confirmed his choice in the popup dialog");
					if (actionYes != null)
					{
						actionYes();
					}
					break;
				case 1:
				default:
					Log.i(TAG, "User has cancelled his choice in the popup dialog");
					if (actionNo != null)
					{
						actionNo();
					}
					break;
				case 2:
					Log.i(TAG, "User has chosen the third choice in the popup dialog");
					actionThird();
					break;
			}
		}
		//Most used overload with only yes / no choices (the only applicable options on mobile) for convenience
		public async static Task ShowPopupDialog(string question, string yes, PopupDialogActionDelegate actionYes, string no, PopupDialogActionDelegate actionNo)
		{
			await ShowPopupDialog(question, yes, actionYes, no, actionNo, false, null, null);
		}
	}
}
