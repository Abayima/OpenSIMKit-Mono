using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

namespace OpenSIMKit.Utilities
{
	public class SerialPortUtility
	{
		// Constants
		const char CTRL_Z = (char)26;
		const String CMD_CHECK_CONNECTION = "AT\r";
		const String CMD_SET_TEXT_MODE_FORMAT = "AT+CMGF=1\r";
		const String CMD_CHAR_SET_PCCP347 = "AT+CSCS=\"PCCP437\"\r";
		const String CMD_SELECT_SIM_STORAGE = "AT+CPMS=\"SM\"\r";
		const String CMD_READ_ALL_MESSAGES = "AT+CMGL=\"REC READ\"\r";
		const String CMD_READ_MESSAGE = "AT+CMGR={{ message_index }}\r";
		const String CMD_DETAILED_ERRORS = "AT+CMEE=1";

		// Full write command
		const String CMD_WRITE_MESSAGE_TO_MEMORY = "AT+CMGW=\"{{ message_contact }}\"\r{{ message }}";

		// Parial write command
		const String CMD_REQUEST_WRITE_MESSAGE = "AT+CMGW=\"{{ message_contact }}\"\r";
		const String CMD_DO_WRITE_AFTER_REQUEST = "{{ message }}";

		List<string> Readers;
		
		// Variables
		private SerialPort mySerialPort;
		private string CurrentCommand = "";
		
		// Constructor - If no serial port connection is sent

		public SerialPortUtility ()
		{
			mySerialPort = new SerialPort("", 115200);
			Readers = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
			mySerialPort.ReadTimeout = 5000;
		}

		// Constructor - for an external serial port connection

		public SerialPortUtility(SerialPort port)
		{
			mySerialPort = port;
		}
		
		// Destructor
		~SerialPortUtility()
		{
			CloseSerialPort ();
		}
		
		// Open up the serial port
		public void OpenSerialPort (int ReaderIndex)
		{
			if(!mySerialPort.IsOpen) {
				mySerialPort.PortName = Readers[ReaderIndex];
				mySerialPort.Open ();
			}
		}
		
		// Close the serial port
		public void CloseSerialPort ()
		{
			if(mySerialPort.IsOpen)
				mySerialPort.Close ();
			
		}

		private void FlushBuffers()
		{
			mySerialPort.DiscardInBuffer();
			mySerialPort.DiscardOutBuffer();
		}
		
		// Runs a custom command
		public String RunCustomCommand(String Command)
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			mySerialPort.Write (Command);
			CurrentCommand = Command;
			Thread.Sleep(150);
			String Response = mySerialPort.ReadExisting();
			
			return Response;
		}
		
		// Check connection status
		public Boolean CheckConnectionStatus()
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			mySerialPort.Write (CMD_CHECK_CONNECTION);
			CurrentCommand = CMD_CHECK_CONNECTION;
			Thread.Sleep(150);
			String Response = mySerialPort.ReadExisting();
			
			if(!Response.Contains ("OK"))
				return false;
			
			return true;
		}
		
		// Read all messages
		public String ReadAllMessages()
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			mySerialPort.Write (CMD_READ_ALL_MESSAGES);
			CurrentCommand = CMD_READ_ALL_MESSAGES;
			Thread.Sleep(150);
			String Response = mySerialPort.ReadExisting();
			
			return Response;
		}

		// Read a message at a specific index
		public String ReadMessage(int MessageIndex)
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			String Command_Set_Text_Format = CMD_SET_TEXT_MODE_FORMAT;
			String Command_Set_Sim_Storage = CMD_SELECT_SIM_STORAGE;

			mySerialPort.Write (Command_Set_Text_Format);
			Thread.Sleep(150);
			String Response = mySerialPort.ReadExisting();

			mySerialPort.Write (Command_Set_Sim_Storage);
			Thread.Sleep(150);
			Response = mySerialPort.ReadExisting();

			String Command = CMD_READ_MESSAGE.Replace("{{ message_index }}", Convert.ToString (MessageIndex));
			CurrentCommand = Command;
			
			mySerialPort.Write (Command);
			Thread.Sleep(150);
			Response = mySerialPort.ReadExisting();
			
			return Response;
		}
		
		// Store a message
		public String StoreMessage (String Contact, String Message)
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			String Command_Set_Text_Format = CMD_SET_TEXT_MODE_FORMAT;
			String Command_Set_Sim_Storage = CMD_SELECT_SIM_STORAGE;
			String Command_Request_Write = CMD_REQUEST_WRITE_MESSAGE.Replace("{{ message_contact }}", "+254722773772");
			String Command_Write_Message = CMD_DO_WRITE_AFTER_REQUEST.Replace("{{ message }}", Message);
			Command_Write_Message = Command_Write_Message + (char) 26;

			mySerialPort.Write (Command_Set_Text_Format);
			Thread.Sleep(150);
			String Response = mySerialPort.ReadExisting();

			mySerialPort.Write (Command_Set_Sim_Storage);
			Thread.Sleep(150);
			Response = mySerialPort.ReadExisting();

			mySerialPort.Write (Command_Request_Write);
			Thread.Sleep(150);
			Response = mySerialPort.ReadExisting();

			if(Response.Contains(">"))
			{
				mySerialPort.Write (Command_Write_Message);
				Thread.Sleep(150);
				Response = mySerialPort.ReadExisting();
			}
			
			return Response;
		}

		// Current Running Command Property

		public string CurrentRunningCommand
		{
			get { return CurrentCommand; }
		}
	}
}

