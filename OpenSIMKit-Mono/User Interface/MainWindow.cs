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

		private SmartMouseReader smReader = new SmartMouseReader();
		private PcscReader pReader = new PcscReader();
		
		private enum SelectedConnectionType {SerialPortConnection, PCSCConnection};
		
		private SelectedConnectionType connectionType;
		private bool connectionActive = false;

		private string [] commandLineArgs;

		private bool messageListPopulated = false;

		private List<string> messages = new List<string>();

		// Constructor and Destructor

		public MainWindow (string[] arg)
		{
			Glade.XML gxml = new Glade.XML(null, "OpenSIMKitMono.glade-gui.MainWindow.glade", "MainDialogWindow", null);
			gxml.Autoconnect(this);
			InitializeControls();

			commandLineArgs = arg;
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
		Button SaveToPCButton;

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
		TreeViewColumn tvMessageColumn;
		ListStore messageListStore = new ListStore(typeof(string));
		CellRendererText messageCellText = new CellRendererText();

		// Utility functions

		private void SetComboBoxIndex(ref ComboBox cb, int index)
		{
			Gtk.TreeIter Iter;
			cb.Model.IterNthChild(out Iter, index);
			cb.SetActiveIter(Iter);
		}

		private void AddItemToComboBox(ref ComboBox cb, int index, string item)
		{
			Gtk.TreeIter Iter;
			cb.Model.IterNthChild(out Iter, index);
			cb.Model.SetValue (Iter, 0, item);
		}

		public static byte[] StringToByteArray(String hexString)
		{
			int NumberChars = hexString.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
			}
			return bytes;
		}

		// Populates the list of serial ports

		private void PopulateSerialPortList()
		{
			List<string> serialReaders = smReader.Readers;
			string [] readersString = serialReaders.ToArray();

			ListStore comboBoxStore = new ListStore (typeof (string)); 

			foreach(string ReaderString in readersString)
			{
				comboBoxStore.AppendValues(ReaderString);
			}

			CellRendererText ct = new CellRendererText(); 
			SerialPortComboBox.PackStart(ct, false); 
			SerialPortComboBox.AddAttribute(ct, "text", 0); 

			SerialPortComboBox.Model = comboBoxStore;
		}

		// Populates the list of PCSC readers

		private void PopulatePCSCReaderList()
		{
			List<string> pcscReaders = pReader.Readers;
			string [] readersString = pcscReaders.ToArray();
			
			ListStore comboBoxStore = new ListStore (typeof (string)); 
			
			foreach(string ReaderString in readersString)
			{
				comboBoxStore.AppendValues(ReaderString);
			}
			
			CellRendererText ct = new CellRendererText(); 
			PCSCReaderComboBox.PackStart(ct, false); 
			PCSCReaderComboBox.AddAttribute(ct, "text", 0); 
			
			PCSCReaderComboBox.Model = comboBoxStore;
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

			tvMessageColumn = new TreeViewColumn();
			tvMessageColumn.Title = "Messages";

			MessagesTreeView.AppendColumn(tvMessageColumn);
			MessagesTreeView.Model = messageListStore;
			tvMessageColumn.PackStart (messageCellText, true);
			tvMessageColumn.AddAttribute(messageCellText, "text", 0);

			// Relevant sizes
			MainDialogWindow.SetSizeRequest(1000, 500);
		}

		// Widget event handlers

		// Connect button clicked

		public void ConnectButton_Clicked(System.Object obj, EventArgs args)
		{
			if(!connectionActive) {
				
				// Instantiate a connection
				
				if(SerialPortRadioButton.Active) {
					connectionType = SelectedConnectionType.SerialPortConnection;
					
					// Set values for the Smart Mouse Reader to use
					
					smReader.PortName = SerialPortComboBox.ActiveText.Trim();
					smReader.PortSpeed = Convert.ToInt32(BitsPerSecondComboBox.ActiveText.Trim());
					smReader.PortDataBit = Convert.ToInt32(DataBitsComboBox.ActiveText.Trim());
					smReader.PortParity = ParityComboBox.ActiveText.Trim();
					smReader.PortStopBit = Convert.ToInt32(StopBitsComboBox.ActiveText.Trim());

					smReader.PortObject.DtrEnable = true;
					smReader.PortObject.RtsEnable = true;
					smReader.PortObject.Handshake = System.IO.Ports.Handshake.RequestToSend;
					
					smReader.ApplySettings();

					// Connect
					Console.WriteLine("Connecting to " + smReader.PortName + "...");

					string ConnectionResponse = "";

					smReader.AnswerToReset(ref ConnectionResponse);

					Console.WriteLine("AnswerToReset response: " + ConnectionResponse);
				}
				else if(PCSCReaderRadioButton.Active) {
					connectionType = SelectedConnectionType.PCSCConnection;
				}
				
				connectionActive = true;
				
				SerialPortRadioButton.Sensitive = false;
				PCSCReaderRadioButton.Sensitive = false;
				
				ConnectButton.Label = "Disconnect";
			}
			else {
				
				// Close up the connection
				
				switch(connectionType) {
				case (SelectedConnectionType.SerialPortConnection):
					
					if(smReader.IsPortOpen)
					{
						smReader.CloseConnection();

						Console.WriteLine("Disconnected from " + smReader.PortName);
					}
					
					break;
				case(SelectedConnectionType.PCSCConnection):
					
					pReader.CloseConnection();
					
					break;
				}
				
				connectionActive = false;
				SerialPortRadioButton.Sensitive = true;
				PCSCReaderRadioButton.Sensitive = true;
				
				ConnectButton.Label = "Connect";
			}
		}

		// Save Config button clicked

		public void SaveConfigButton_Clicked(System.Object obj, EventArgs args)
		{
			// TODO: Save config
		}

		// Gets messages from SIM Card
		
		public void GetSIMMessagesButton_Clicked(System.Object obj, EventArgs args)
		{
			if(connectionActive)
			{
				switch(connectionType) {
				case SelectedConnectionType.SerialPortConnection:
					SerialPortUtility SerialUtility = new SerialPortUtility(smReader.PortObject);
					
					int CurrentMessage = 1;
					bool ReadStatus = true;

					messageListPopulated = false;
					
					do {
						String Message = SerialUtility.ReadMessage(CurrentMessage);
						SMSUtilities SMSUtility = new SMSUtilities(Message, SMSUtilities.Direction.SMS_IN);
						
						string ProcessedMessage = SMSUtility.ProcessedMessage;
						
						if(ProcessedMessage == null)
						{
							// Error encountered. Reached the end
							ReadStatus = false;
						}
						else if(ProcessedMessage.Trim().Equals(SerialUtility.CurrentCommand.Trim()))
						{
							// Get only stored messages
							ReadStatus = false;
						}
						else 
						{
							// Add the item
							messages.Add (ProcessedMessage);
						}
						
						CurrentMessage ++;
					}
					while(ReadStatus);

					messageListPopulated = true;

					break;
				case SelectedConnectionType.PCSCConnection:
					// TODO: Implement PCSC functionality for the messages list
					break;
				}
				

				// Populate messages

				foreach(string IndividualMessage  in messages)
				{
					messageListStore.AppendValues(IndividualMessage);
				}
			}
		}

		// Manage PC Messages Button Clicked
		
		public void ManagePCMessagesButton_Clicked(System.Object obj, EventArgs args)
		{
			MessageManager MessageManagerDialog = new MessageManager(commandLineArgs);
		}

		// Save to PC button clicked

		public void SaveToPCButton_Clicked(System.Object obj, EventArgs args)
		{
			if(!messageListPopulated)
			{
				GetSIMMessagesButton_Clicked(obj, args);
			}

			string ContactFrom = "OpenSIMKit";

			XMLUtilities xmlUtilities = new XMLUtilities();
			xmlUtilities.SaveMessagesXMLFile(ContactFrom, messages); 
		}

		// Copy from PC button clicked

		public void CopyFromPCButton_Clicked(System.Object obj, EventArgs args)
		{

			if(connectionActive)
			{
				XMLUtilities xmlUtilites = new XMLUtilities();
				List<string> messages;
				string contact;
				
				xmlUtilites.LoadMessagesXMLFile();
				messages = xmlUtilites.StringArray;
				contact = xmlUtilites.ContactText;

				switch(connectionType)
				{
				case SelectedConnectionType.SerialPortConnection:
					if(contact != null && messages != null) 
					{
						SerialPortUtility MySerialPortUtility = new SerialPortUtility(smReader.PortObject);

						foreach(string Message in messages) 
						{
							// Save these messages to SIM card
							MySerialPortUtility.StoreMessage(contact, Message);
						}
					}
					break;

				case SelectedConnectionType.PCSCConnection:
					break;
				}
			}
		}

		// Execute an AT Command

		public void ExecuteButton_Clicked(System.Object obj, EventArgs args)
		{
			if(connectionActive) {
				switch(connectionType) {
				case SelectedConnectionType.SerialPortConnection:
					string CommandResult = "";
					string Command = "";

					Command = CommandText.Text.Trim() + "\r";

					SerialPortUtility SerialUtility = new SerialPortUtility(smReader.PortObject);
					CommandResult = SerialUtility.RunCustomCommand(Command);

					ResultsTextView.Buffer.Text = CommandResult;

					break;
				case SelectedConnectionType.PCSCConnection:
					// TODO: Implement functionality for the PCSC readers
					break;
				}
			}
		}
	}
}

