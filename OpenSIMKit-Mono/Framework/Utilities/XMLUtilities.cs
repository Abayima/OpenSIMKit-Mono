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

		private XmlDocument MyXmlDocument;
		private XmlNode ContactDataNode;
		private XmlNode MessagesNode;
		private string ContactText;
		private string [] StringArray;

		private string ExecutablePath;

		private const string MessagesFile = "Messages.xml";
		private const string MessageDataTag = "messagedata";
		private const string MessageSourceTag = "messagesource";
		private const string MessageTag = "message";
		private const string MessagesTag = "messages";
		private const string ContactTag = "contact";

		// Constructor

		public XMLUtilities ()
		{
			ExecutablePath = Assembly.GetExecutingAssembly().Location;

			CreateXMLFile ();
		}

		// Functions

		private void CreateXMLFile()
		{
			FileInfo XMLFile = new FileInfo(ExecutablePath + "-" + MessagesFile);
			
			if(!XMLFile.Exists)
			{
				StreamWriter SW = XMLFile.CreateText();
				SW.WriteLine("<?xml version='1.0'?>");
				SW.WriteLine("<" + MessageDataTag + ">");
				SW.WriteLine("</" + MessageDataTag + ">");
				SW.Close();
			}
		}

		public void SaveMessagesXMLFile(string ContactText, List<string>Messages)
		{
			XmlWriter MyXmlWriter = XmlWriter.Create(ExecutablePath + "-" + MessagesFile);
			
			MyXmlWriter.WriteStartElement (MessageDataTag);
			
			MyXmlWriter.WriteStartElement (ContactTag);
			MyXmlWriter.WriteValue(ContactText);
			MyXmlWriter.WriteEndElement();
			
			MyXmlWriter.WriteStartElement (MessagesTag);
			
			int Count = 1;
			
			foreach(string MessageRow in Messages)
			{
				MyXmlWriter.WriteStartElement(MessageTag + "_" + Count.ToString());
				MyXmlWriter.WriteValue (MessageRow);
				MyXmlWriter.WriteEndElement();
				
				Count ++;
			}
			
			MyXmlWriter.WriteEndElement();
			MyXmlWriter.WriteEndElement();
			
			MyXmlWriter.WriteEndDocument();
			MyXmlWriter.Flush();
			
			MyXmlWriter.Close();
		}

		public void LoadMessagesXMLFile()
		{
			MyXmlDocument = new XmlDocument();
			MyXmlDocument.Load (ExecutablePath + "-" + MessagesFile);
			ContactDataNode = MyXmlDocument.SelectSingleNode("//" +MessageDataTag + "/" + ContactTag);
			MessagesNode = MyXmlDocument.SelectSingleNode("//" +MessageDataTag + "/" + MessagesTag);

			if(ContactDataNode != null)
			{
				ContactText = ContactDataNode.InnerText;
			}
			else
			{
				ContactText = null;
			}
			
			LoadMessagesXMLToStringArray();
		}

		private void LoadMessagesXMLToStringArray()
		{
			if(MessagesNode != null)
			{
				StringArray = new String[MessagesNode.ChildNodes.Count];

				int CurrentArrayItem = 0;

				foreach(XmlNode MessageNode in MessagesNode)
				{
					StringArray[CurrentArrayItem] = MessageNode.InnerText;
					CurrentArrayItem ++;
				}
			}
			else
			{
				StringArray = null;
			}
		}

		// Properties

		public string [] TheStringArray
		{
			get { return StringArray; }
		}

		public string TheContactText
		{
			get { return ContactText; }
		}
	}
}

