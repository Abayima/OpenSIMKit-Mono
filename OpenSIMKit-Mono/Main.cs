using System;
using Gtk;

namespace OpenSIMKitMono
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Gtk.Application.Init();
			new MainWindow(args);
			Gtk.Application.Run ();
		}
	}
}
