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
		const String CMD_WRITE_MESSAGE_TO_MEMORY = "AT+CMGW=\"+254772011011\"\r\n{{ message }}\u001F\r";
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

			String Command = CMD_READ_MESSAGE.Replace("{{ message_index }}", Convert.ToString (MessageIndex));
			CurrentCommand = Command;
			
			mySerialPort.Write (Command);
			Thread.Sleep(150);
			String Response = mySerialPort.ReadExisting();
			
			return Response;
		}
		
		// Store a message
		public String StoreMessage (String Message)
		{
			if(!mySerialPort.IsOpen)
				throw new SerialPortUtilityException("Serial port is not opened yet");

			FlushBuffers();
			Thread.Sleep(150);

			String Command = CMD_WRITE_MESSAGE_TO_MEMORY.Replace("{{ message }}", Message);
			CurrentCommand = Command;
			
			mySerialPort.Write (Command);
			Thread.Sleep(150);
			String Response = mySerialPort.ReadExisting();
			
			return Response;
		}

		// Current Running Command Property

		public string CurrentRunningCommand
		{
			get { return CurrentCommand; }
		}
	}
}

