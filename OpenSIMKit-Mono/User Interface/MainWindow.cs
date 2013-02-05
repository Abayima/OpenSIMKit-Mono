using System;
using Glade;
using Gtk;
using comexbase;
using System.Collections.Generic;
using OpenSIMKit.Utilities;

namespace OpenSIMKitMono
{
	public class MainWindow
	{
		// Private variables

		private SmartMouseReader SMReader = new SmartMouseReader();
		private PcscReader PReader = new PcscReader();
		
		private enum SelectedConnectionType {SerialPortConnection, PCSCConnection};
		
		private SelectedConnectionType ConnectionType;
		private bool ConnectionActive = false;

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
		Button ConnectButton;

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

		[Widget]
		Entry CommandText;

		[Widget]
		TextView ResultsTextView;
		TreeViewColumn TVMessageColumn;
		ListStore MessageListStore = new ListStore(typeof(string));
		CellRendererText MessageCellText = new CellRendererText();

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

		public static byte[] StringToByteArray(String HexString)
		{
			int NumberChars = HexString.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
			}
			return bytes;
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
			// Serial ports list and PCSC Readers list

			PopulateSerialPortList();
			PopulatePCSCReaderList();

			// Combo boxes

			SetComboBoxIndex(ref BitsPerSecondComboBox, 6);
			SetComboBoxIndex(ref DataBitsComboBox, 3);
			SetComboBoxIndex(ref ParityComboBox, 0);
			SetComboBoxIndex(ref StopBitsComboBox, 0);
			SetComboBoxIndex(ref FlowControlComboBox, 2);

			// Tree view

			TVMessageColumn = new TreeViewColumn();
			TVMessageColumn.Title = "Messages";

			MessagesTreeView.AppendColumn(TVMessageColumn);
			MessagesTreeView.Model = MessageListStore;
			TVMessageColumn.PackStart (MessageCellText, true);
			TVMessageColumn.AddAttribute(MessageCellText, "text", 0);

			// Relevant sizes
			MainDialogWindow.SetSizeRequest(1000, 500);
		}

		// Widget event handlers

		// Connect button clicked

		public void ConnectButton_Clicked(System.Object Obj, EventArgs args)
		{
			if(!ConnectionActive) {
				
				// Instantiate a connection
				
				if(SerialPortRadioButton.Active) {
					ConnectionType = SelectedConnectionType.SerialPortConnection;
					
					// Set values for the Smart Mouse Reader to use
					
					SMReader.PortName = SerialPortComboBox.ActiveText.Trim();
					SMReader.PortSpeed = Convert.ToInt32(BitsPerSecondComboBox.ActiveText.Trim());
					SMReader.PortDataBit = Convert.ToInt32(DataBitsComboBox.ActiveText.Trim());
					SMReader.PortParity = ParityComboBox.ActiveText.Trim();
					SMReader.PortStopBit = Convert.ToInt32(StopBitsComboBox.ActiveText.Trim());
					
					SMReader.ApplySettings();

					// Connect
					Console.WriteLine("Connecting to " + SMReader.PortName + "...");

					string ConnectionResponse = "";

					SMReader.AnswerToReset(ref ConnectionResponse);

					Console.WriteLine("AnswerToReset response: " + ConnectionResponse);
				}
				else if(PCSCReaderRadioButton.Active) {
					ConnectionType = SelectedConnectionType.PCSCConnection;
				}
				
				ConnectionActive = true;
				
				SerialPortRadioButton.Sensitive = false;
				PCSCReaderRadioButton.Sensitive = false;
				
				ConnectButton.Label = "Disconnect";
			}
			else {
				
				// Close up the connection
				
				switch(ConnectionType) {
				case (SelectedConnectionType.SerialPortConnection):
					
					if(SMReader.IsPortOpen)
					{
						SMReader.CloseConnection();

						Console.WriteLine("Disconnected from " + SMReader.PortName);
					}
					
					break;
				case(SelectedConnectionType.PCSCConnection):
					
					PReader.CloseConnection();
					
					break;
				}
				
				ConnectionActive = false;
				SerialPortRadioButton.Sensitive = true;
				PCSCReaderRadioButton.Sensitive = true;
				
				ConnectButton.Label = "Connect";
			}
		}

		// Save Config button clicked

		public void SaveConfigButton_Clicked(System.Object Obj, EventArgs args)
		{
			// TODO: Save config
		}

		// Gets messages from SIM Card
		
		public void GetSIMMessagesButton_Clicked(System.Object Obj, EventArgs args)
		{
			List<string> Messages = new List<string>();

			if(ConnectionActive)
			{
				SerialPortUtility SerialUtility = new SerialPortUtility(SMReader.PortObject);

				int CurrentMessage = 1;
				bool ReadStatus = true;

				do {
					String Message = SerialUtility.ReadMessage(CurrentMessage);
					SMSUtilities SMSUtility = new SMSUtilities(Message, SMSUtilities.Direction.SMS_IN);

					string ProcessedMessage = SMSUtility.ProcessedMessageText;

					if(ProcessedMessage == null)
					{
						// Error encountered. Reached the end
						ReadStatus = false;
					}
					else if(ProcessedMessage.Trim().Equals(SerialUtility.CurrentRunningCommand.Trim()))
					{
						// Get only stored messages
						ReadStatus = false;
					}
					else 
					{
						// Add the item
						Messages.Add (ProcessedMessage);
					}

					CurrentMessage ++;
				}
				while(ReadStatus);

				foreach(string IndividualMessage  in Messages)
				{
					MessageListStore.AppendValues(IndividualMessage);
				}
			}
		}

		// Save to PC button clicked

		public void SaveToPCButton_Clicked(System.Object Obj, EventArgs args)
		{
			
		}

		// Copy from PC button clicked

		public void CopyFromPCButton_Clicked(System.Object Obj, EventArgs args)
		{
			
		}

		// Execute an AT Command

		public void ExecuteButton_Clicked(System.Object Obj, EventArgs args)
		{
			if(ConnectionActive) {
				switch(ConnectionType) {
				case SelectedConnectionType.SerialPortConnection:
					string CommandResult = "";
					string Command = "";

					Command = CommandText.Text.Trim() + "\r";

					SerialPortUtility SerialUtility = new SerialPortUtility(SMReader.PortObject);
					CommandResult = SerialUtility.RunCustomCommand(Command);

					ResultsTextView.Buffer.Text = CommandResult;

					break;
				case SelectedConnectionType.PCSCConnection:
					break;
				}
			}
		}
	}
}

