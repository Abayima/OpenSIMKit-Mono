using System;

namespace OpenSIMKit.Utilities
{
	public class SMSUtilities
	{
		public enum Direction {SMS_IN, SMS_OUT};
		private Direction CurrentMessageDirection;
		private string MessageString = "";
		private string ProcessedMessage = "";

		public SMSUtilities (string Message, Direction MessageDirection)
		{
			MessageString = Message;

			switch(MessageDirection) {
			case(Direction.SMS_IN):
				CurrentMessageDirection = Direction.SMS_IN;
				ProcessedMessage = ProcessINMessage();
				break;
			case(Direction.SMS_OUT):
				CurrentMessageDirection = Direction.SMS_OUT;
				break;
			}
		}

		private string ProcessINMessage()
		{
			char [] Seperators = new char []{'\r'};
			string [] SplitMessages = MessageString.Split (Seperators);
			int NumMessages = SplitMessages.GetUpperBound(0) + 1;
			string MessageText = "";
			int StringPosition = 4;

			for(int SplitsLoop = (NumMessages - 1); SplitsLoop >= 0; SplitsLoop --)
			{
				string TrimmedMessage = SplitMessages[SplitsLoop].Trim ();
				if(TrimmedMessage != "")
				{
					switch(StringPosition) {
					case 4:
						if(TrimmedMessage == "ERROR")
							return null;
						else if(TrimmedMessage == "OK")
							StringPosition --;
						break;

					case 3:
						ProcessedMessage = TrimmedMessage;
						StringPosition --;

						return ProcessedMessage;
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
		
		public string ConvertHexToString(string HexValue)
		{
			string StrValue = "";
			while (HexValue.Length > 0)
			{
				StrValue += System.Convert.ToChar(System.Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString();
				HexValue = HexValue.Substring(2, HexValue.Length - 2);
			}
			return StrValue;
		}

		public string ProcessedMessageText
		{
			get { return ProcessedMessage; }
		}
	}
}

