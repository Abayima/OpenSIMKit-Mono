using System;
using Glade;
using Gtk;
using comexbase;
using System.Collections.Generic;

namespace OpenSIMKitMono
{
	public class MainWindow
	{
		// Private variables

		private SmartMouseReader SMReader = new SmartMouseReader();
		private PcscReader PReader = new PcscReader();

		// Constructor and Destructor

		public MainWindow (string[] arg)
		{
			Glade.XML gxml = new Glade.XML(null, "OpenSIMKitMono.glade-gui.MainWindow.glade", "MainDialogWindow", null);
			gxml.Autoconnect(this);
			InitializeControls();
		}

		// Frame handler

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

		// Initialize controls

		[Widget]
		Dialog MainDialogWindow;

		[Widget]
		RadioButton SerialPortRadioButton;

		[Widget]
		RadioButton PCSCReaderRadioButton;

		[Widget]
		ComboBox SerialPortComboBox;

		[Widget]
		ComboBox PCSCReaderComboBox;

		[Widget]
		TreeView MessagesTreeView;

		[Widget]
		Button SaveTPCButton;

		[Widget]
		Button CopyFromPCButton;

		[Widget]
		Button SaveConfigButton;

		[Widget]
		Button ExitButton;

		[Widget]
		ComboBox BitsPerSecondComboBox;

		[Widget]
		ComboBox DataBitsComboBox;

		[Widget]
		ComboBox ParityComboBox;

		[Widget]
		ComboBox StopBitsComboBox;

		[Widget]
		ComboBox FlowControlComboBox;

		// Utility functions

		private void SetComboBoxIndex(ref ComboBox CB, int Index)
		{
			Gtk.TreeIter Iter;
			CB.Model.IterNthChild(out Iter, Index);
			CB.SetActiveIter(Iter);
		}

		private void AddItemToComboBox(ref ComboBox CB, int Index, string Item)
		{
			Gtk.TreeIter Iter;
			CB.Model.IterNthChild(out Iter, Index);
			CB.Model.SetValue (Iter, 0, Item);
		}

		// Populates the list of serial ports

		private void PopulateSerialPortList()
		{
			List<string> SerialReaders = SMReader.Readers;
			string [] ReadersString = SerialReaders.ToArray();

			ListStore ComboBoxStore = new ListStore (typeof (string)); 

			foreach(string ReaderString in ReadersString)
			{
				ComboBoxStore.AppendValues(ReaderString);
			}

			CellRendererText CT = new CellRendererText(); 
			SerialPortComboBox.PackStart(CT, false); 
			SerialPortComboBox.AddAttribute(CT, "text", 0); 

			SerialPortComboBox.Model = ComboBoxStore;
		}

		// Populates the list of PCSC readers

		private void PopulatePCSCReaderList()
		{
			List<string> PCSCReaders = PReader.Readers;
			string [] ReadersString = PCSCReaders.ToArray();
			
			ListStore ComboBoxStore = new ListStore (typeof (string)); 
			
			foreach(string ReaderString in ReadersString)
			{
				ComboBoxStore.AppendValues(ReaderString);
			}
			
			CellRendererText CT = new CellRendererText(); 
			PCSCReaderComboBox.PackStart(CT, false); 
			PCSCReaderComboBox.AddAttribute(CT, "text", 0); 
			
			PCSCReaderComboBox.Model = ComboBoxStore;
		}

		// Initializes all controls

		private void InitializeControls ()
		{
			PopulateSerialPortList();
			PopulatePCSCReaderList();

			SetComboBoxIndex(ref BitsPerSecondComboBox, 6);
			SetComboBoxIndex(ref DataBitsComboBox, 3);
			SetComboBoxIndex(ref ParityComboBox, 0);
			SetComboBoxIndex(ref StopBitsComboBox, 0);
			SetComboBoxIndex(ref FlowControlComboBox, 2);

			// Relevant sizes
			MainDialogWindow.SetSizeRequest(1000, 500);
		}

		// Widget event handlers

		// Connect button clicked

		public void ConnectButton_Clicked(System.Object Obj, EventArgs args)
		{

		}

		// Save Config button clicked

		public void SaveConfigButton_Clicked(System.Object Obj, EventArgs args)
		{
			
		}

		// Save to PC button clicked

		public void SaveToPCButton_Clicked(System.Object Obj, EventArgs args)
		{
			
		}

		// Copy from PC button clicked

		public void CopyFromPCButton_Clicked(System.Object Obj, EventArgs args)
		{
			
		}
	}
}

