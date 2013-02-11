using System;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace OpenSIMKit.Utilities
{
	public class XMLUtilities
	{

		// Variables

		private XmlDocument xmlDocument;
		private XmlNode contactDataNode;
		private XmlNode messagesNode;
		private string contactText;
		private List<string> stringArray = new List<string>();

		private string executablePath;

		private const string messagesFile = "Messages.xml";
		private const string messageDataTag = "messagedata";
		private const string messageSourceTag = "messagesource";
		private const string messageTag = "message";
		private const string messagesTag = "messages";
		private const string contactTag = "contact";

		// Constructor

		public XMLUtilities ()
		{
			executablePath = Assembly.GetExecutingAssembly().Location;

			CreateXMLFile ();
		}

		// Functions

		private void CreateXMLFile()
		{
			FileInfo xmlFile = new FileInfo(executablePath + "-" + messagesFile);
			
			if(!xmlFile.Exists)
			{
				StreamWriter sw = xmlFile.CreateText();
				sw.WriteLine("<?xml version='1.0'?>");
				sw.WriteLine("<" + messageDataTag + ">");
				sw.WriteLine("</" + messageDataTag + ">");
				sw.Close();
			}
		}

		public void SaveMessagesXMLFile(string contactText, List<string>messages)
		{
			XmlWriter xmlWriter = XmlWriter.Create(executablePath + "-" + messagesFile);
			
			xmlWriter.WriteStartElement (messageDataTag);
			
			xmlWriter.WriteStartElement (contactTag);
			xmlWriter.WriteValue(contactText);
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteStartElement (messagesTag);
			
			int Count = 1;
			
			foreach(string MessageRow in messages)
			{
				xmlWriter.WriteStartElement(messageTag + "_" + Count.ToString());
				xmlWriter.WriteValue (MessageRow);
				xmlWriter.WriteEndElement();
				
				Count ++;
			}
			
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			
			xmlWriter.Close();
		}

		public void LoadMessagesXMLFile()
		{
			xmlDocument = new XmlDocument();
			xmlDocument.Load (executablePath + "-" + messagesFile);
			contactDataNode = xmlDocument.SelectSingleNode("//" +messageDataTag + "/" + contactTag);
			messagesNode = xmlDocument.SelectSingleNode("//" +messageDataTag + "/" + messagesTag);

			if(contactDataNode != null)
			{
				contactText = contactDataNode.InnerText;
			}
			else
			{
				contactText = null;
			}
			
			LoadMessagesXMLToStringArray();
		}

		private void LoadMessagesXMLToStringArray()
		{
			if(messagesNode != null)
			{
				int CurrentArrayItem = 0;

				foreach(XmlNode MessageNode in messagesNode)
				{
					stringArray.Add (MessageNode.InnerText);
					CurrentArrayItem ++;
				}
			}
			else
			{
				stringArray = null;
			}
		}

		// Properties

		public List<string> StringArray
		{
			get { return stringArray; }
		}

		public string ContactText
		{
			get { return contactText; }
		}
	}
}

