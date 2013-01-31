using System;

// This will replace the default logger for a multiplatform logging solution

namespace comexbase
{
	public class ComexLog
	{
		public ComexLog ()
		{
		}
		
		/// <summary>
		/// This is to be the customized logging engine
		/// </summary>
		
		private void LogEntry(String LogType, String LogEntry)
		{
			Console.WriteLine(LogType + ": " + LogEntry);
		}
		
		public void Error(String MyLogEntry)
		{
			LogEntry("Error", MyLogEntry);
		}
		
		public void Debug(String MyLogEntry)
		{
			LogEntry("Debug", MyLogEntry);
		}
		
		public void Info(String MyLogEntry)
		{
			LogEntry("Info", MyLogEntry);
		}
	}
}

