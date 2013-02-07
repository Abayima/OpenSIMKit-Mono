using System;
using System.IO;
using System.Xml;
using Glade;
using Gtk;
using System.Collections.Generic;
using System.Reflection;

namespace OpenSIMKitMono
{
	public class MessageManager
	{

		// Member variables

		private XmlDocument MyXmlDocument;
		private string ContactText;
		private XmlNode ContactDataNode;
		private XmlNode MessagesNode;
		private string ExecutablePath = "";

		private TreeViewColumn MessagesColumn = new TreeViewColumn();
		private ListStore MessagesListStore = new ListStore(typeof(string));
		private CellRendererText MessageCellText = new CellRendererText();

		// Constants

		private const string MessagesFile = "Messages.xml";
		private const string MessageDataTag = "messagedata";
		private const string MessageSourceTag = "messagesource";
		private const string MessageTag = "message";
		private const string MessagesTag = "messages";
		private const string ContactTag = "contact";

		// Constructors

		public MessageManager (string [] arg)
		{
			Glade.XML gxml = new Glade.XML(null, "OpenSIMKitMono.glade-gui.MessageManager.glade", "MessagesDialog", null);
			gxml.Autoconnect(this);

			ExecutablePath = Assembly.GetExecutingAssembly().Location;

			// Create the file if it does not exist

			CreateXMLFile();

			// Load the XML document

			MyXmlDocument = new XmlDocument();
			MyXmlDocument.Load (ExecutablePath + "-" + MessagesFile);
			ContactDataNode = MyXmlDocument.SelectSingleNode("//" +MessageDataTag + "/" + ContactTag);
			MessagesNode = MyXmlDocument.SelectSingleNode("//" +MessageDataTag + "/" + MessagesTag);

			InitializeControls();
		}

		// Widgets

		[Widget]
		Dialog MessagesDialog;

		[Widget]
		Button AddButton;

		[Widget]
		Button DeleteButton;

		[Widget]
		Button SaveButton;

		[Widget]
		Button ExitButton;

		[Widget]
		Entry ContactTextEntry;

		[Widget]
		Entry MessageTextEntry;

		[Widget]
		TreeView MessagesTreeView;

		// Utility functions

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

		private void InitializeControls()
		{
			MessagesTreeView.Model = MessagesListStore;
			MessagesColumn.Title = "Messages";
			MessagesTreeView.AppendColumn(MessagesColumn);
			MessagesColumn.PackStart(MessageCellText, true);
			MessagesColumn.AddAttribute(MessageCellText, "text", 0);

			MessagesDialog.SetSizeRequest(1000, 500);
			MessagesDialog.Show();

			LoadXMLFile();
		}

		private void SaveXMLFile()
		{
			ContactText = ContactTextEntry.Text;
			XmlWriter MyXmlWriter = XmlWriter.Create(ExecutablePath + "-" + MessagesFile);

			MyXmlWriter.WriteStartElement (MessageDataTag);

			MyXmlWriter.WriteStartElement (ContactTag);
			MyXmlWriter.WriteValue(ContactText);
			MyXmlWriter.WriteEndElement();

			MyXmlWriter.WriteStartElement (MessagesTag);

			int Count = 1;

			foreach(object [] MessageRow in MessagesListStore)
			{
				MyXmlWriter.WriteStartElement(MessageTag + "_" + Count.ToString());
				MyXmlWriter.WriteValue (MessageRow[0]);
				MyXmlWriter.WriteEndElement();

				Count ++;
			}

			MyXmlWriter.WriteEndElement();
			MyXmlWriter.WriteEndElement();

			MyXmlWriter.WriteEndDocument();
			MyXmlWriter.Flush();

			MyXmlWriter.Close();
		}

		private void LoadXMLFile()
		{
			if(ContactDataNode != null)
			{
				ContactTextEntry.Text = ContactDataNode.InnerText;
				ContactText = ContactDataNode.InnerText;
			}

			LoadXMLToListStore ();
		}

		private void LoadXMLToListStore()
		{
			if(MessagesNode != null)
			{
				foreach(XmlNode MessageNode in MessagesNode)
				{
					MessagesListStore.AppendValues(MessageNode.InnerText);
				}
			}
		}

		private void UpdateItem()
		{

			TreeIter Iter;
			
			TreePath[] MyTreePath = MessagesTreeView.Selection.GetSelectedRows();
			
			for (int i  = MyTreePath.Length; i > 0; i--)
			{
				MessagesListStore.GetIter(out Iter, MyTreePath[(i - 1)]);
				MessagesListStore.SetValues(Iter, MessageTextEntry.Text);
			}
		}

		private void AddItem()
		{
			MessagesListStore.AppendValues(MessageTextEntry.Text);
		}

		private void DeleteItems()
		{
			TreeIter Iter;
			
			TreePath[] MyTreePath = MessagesTreeView.Selection.GetSelectedRows();
			
			for (int i  = MyTreePath.Length; i > 0; i--)
			{
				MessagesListStore.GetIter(out Iter, MyTreePath[(i - 1)]);
				string value = (string)MessagesListStore.GetValue(Iter, 0);
				MessagesListStore.Remove(ref Iter);
			}
		}

		// Events

		public void AddButton_Clicked(System.Object Obj, EventArgs args)
		{
			string MessageToAdd = MessageTextEntry.Text;

			if(MessageToAdd.Trim () != "")
			{
				AddItem();
			}
		}

		public void UpdateButton_Clicked(System.Object Obj, EventArgs args)
		{
			UpdateItem ();
		}

		public void DeleteButton_Clicked(System.Object Obj, EventArgs args)
		{
			DeleteItems ();
		}

		public void SaveButton_Clicked(System.Object Obj, EventArgs args)
		{
			SaveXMLFile ();
		}

		public void ExitButton_Clicked(System.Object Obj, EventArgs args)
		{
			MessagesDialog.Destroy();
		}
	}


}

