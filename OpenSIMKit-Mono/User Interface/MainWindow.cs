using System;
using Glade;
using Gtk;

namespace OpenSIMKitMono
{
	public class MainWindow
	{
		public MainWindow (string[] arg)
		{
			Glade.XML gxml = new Glade.XML(null, "OpenSIMKitMono.glade-gui.MainWindow.glade", "MainWindow", null);
			gxml.Autoconnect(this);
		}

		private void QuitApplication()
		{
			Gtk.Application.Quit();
		}

		public void ExitButton_Clicked(System.Object Obj, EventArgs args)
		{
			QuitApplication();
		}

		public void MainWindow_Close(System.Object Obj, EventArgs args)
		{
			QuitApplication();
		}
	}
}

