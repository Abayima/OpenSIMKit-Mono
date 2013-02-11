using System;

namespace OpenSIMKit.Utilities
{
	public class SMSUtilities
	{
		public enum Direction {SMS_IN, SMS_OUT};
		private Direction currentMessageDirection;
		private string messageString = "";
		private string processedMessage = "";

		public SMSUtilities (string message, Direction messageDirection)
		{
			messageString = message;

			switch(messageDirection) {
			case(Direction.SMS_IN):
				currentMessageDirection = Direction.SMS_IN;
				processedMessage = ProcessINMessage();
				break;
			case(Direction.SMS_OUT):
				currentMessageDirection = Direction.SMS_OUT;
				break;
			}
		}

		private string ProcessINMessage()
		{
			char [] seperators = new char []{'\r'};
			string [] splitMessages = messageString.Split (seperators);
			int numMessages = splitMessages.GetUpperBound(0) + 1;
			string messageText = "";
			int stringPosition = 4;

			for(int splitsLoop = (numMessages - 1); splitsLoop >= 0; splitsLoop --)
			{
				string trimmedMessage = splitMessages[splitsLoop].Trim ();
				if(trimmedMessage != "")
				{
					switch(stringPosition) {
					case 4:
						if(trimmedMessage == "ERROR")
							return null;
						else if(trimmedMessage == "OK")
							stringPosition --;
						break;

					case 3:
						processedMessage = trimmedMessage;
						stringPosition --;

						return processedMessage;
						break;
					}
				}
			}

			return null;
		}

		public string ConvertStringToHex(string asciiString)
		{
			string hex = "";
			foreach (char c in asciiString)
			{
				int tmp = c;
				hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
			}
			return hex;
		}
		
		public string ConvertHexToString(string hexValue)
		{
			string strValue = "";
			while (hexValue.Length > 0)
			{
				strValue += System.Convert.ToChar(System.Convert.ToUInt32(hexValue.Substring(0, 2), 16)).ToString();
				hexValue = hexValue.Substring(2, hexValue.Length - 2);
			}
			return strValue;
		}

		public string ProcessedMessage
		{
			get { return processedMessage; }
		}
	}
}

