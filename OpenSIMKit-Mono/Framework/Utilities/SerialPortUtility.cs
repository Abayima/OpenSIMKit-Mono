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

		List<string> readers;
		
		// Variables
		private SerialPort mySerialPort;
		private string currentCommand = "";
		
		// Constructor - If no serial port connection is sent

		public SerialPortUtility ()
		{
			mySerialPort = new SerialPort("", 115200);
			readers = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
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
		public void OpenSerialPort (int readerIndex)
		{
			if(!mySerialPort.IsOpen) {
				mySerialPort.PortName = readers[readerIndex];
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
		public String RunCustomCommand(String command)
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			mySerialPort.Write (command);
			currentCommand = command;
			Thread.Sleep(150);
			String response = mySerialPort.ReadExisting();
			
			return response;
		}
		
		// Check connection status
		public Boolean CheckConnectionStatus()
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			mySerialPort.Write (CMD_CHECK_CONNECTION);
			currentCommand = CMD_CHECK_CONNECTION;
			Thread.Sleep(150);
			String response = mySerialPort.ReadExisting();
			
			if(!response.Contains ("OK"))
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
			currentCommand = CMD_READ_ALL_MESSAGES;
			Thread.Sleep(150);
			String response = mySerialPort.ReadExisting();
			
			return response;
		}

		// Read a message at a specific index
		public String ReadMessage(int MessageIndex)
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			String commandSetTextFormat = CMD_SET_TEXT_MODE_FORMAT;
			String commandSetSimStorage = CMD_SELECT_SIM_STORAGE;

			mySerialPort.Write (commandSetTextFormat);
			Thread.Sleep(150);
			String response = mySerialPort.ReadExisting();

			mySerialPort.Write (commandSetSimStorage);
			Thread.Sleep(150);
			response = mySerialPort.ReadExisting();

			String command = CMD_READ_MESSAGE.Replace("{{ message_index }}", Convert.ToString (MessageIndex));
			currentCommand = command;
			
			mySerialPort.Write (command);
			Thread.Sleep(150);
			response = mySerialPort.ReadExisting();
			
			return response;
		}
		
		// Store a message
		public String StoreMessage (String Contact, String Message)
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			String commandSetTextFormat = CMD_SET_TEXT_MODE_FORMAT;
			String commandSetSimStorage = CMD_SELECT_SIM_STORAGE;
			String commandRequestWrite = CMD_REQUEST_WRITE_MESSAGE.Replace("{{ message_contact }}", "+254722773772");
			String commandWriteMessage = CMD_DO_WRITE_AFTER_REQUEST.Replace("{{ message }}", Message);
			commandWriteMessage = commandWriteMessage + (char) 26;

			mySerialPort.Write (commandSetTextFormat);
			Thread.Sleep(150);
			String response = mySerialPort.ReadExisting();

			mySerialPort.Write (commandSetSimStorage);
			Thread.Sleep(150);
			response = mySerialPort.ReadExisting();

			mySerialPort.Write (commandRequestWrite);
			Thread.Sleep(150);
			response = mySerialPort.ReadExisting();

			if(response.Contains(">"))
			{
				mySerialPort.Write (commandWriteMessage);
				Thread.Sleep(150);
				response = mySerialPort.ReadExisting();
			}
			
			return response;
		}

		// Current Running Command Property

		public string CurrentCommand
		{
			get { return currentCommand; }
		}
	}
}

