using System;
using System.IO;
using System.Collections.Generic;
using Glade;
using Gtk;
using OpenSIMKit.Utilities;

namespace OpenSIMKitMono
{
	public class MessageManager
	{

		// Member variables

		private XMLUtilities xmlUtilities = new XMLUtilities();

		private TreeViewColumn messagesColumn = new TreeViewColumn();
		private ListStore messagesListStore = new ListStore(typeof(string));
		private CellRendererText messageCellText = new CellRendererText();

		// Constructors

		public MessageManager (string [] arg)
		{
			Glade.XML gxml = new Glade.XML(null, "OpenSIMKitMono.glade-gui.MessageManager.glade", "MessagesDialog", null);
			gxml.Autoconnect(this);

			// Load the XML document

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

		private void InitializeControls()
		{
			MessagesTreeView.Model = messagesListStore;
			messagesColumn.Title = "Messages";
			MessagesTreeView.AppendColumn(messagesColumn);
			messagesColumn.PackStart(messageCellText, true);
			messagesColumn.AddAttribute(messageCellText, "text", 0);

			MessagesDialog.SetSizeRequest(1000, 500);
			MessagesDialog.Show();

			LoadXMLFile();
		}

		private void SaveXMLFile()
		{
			List<string> messages = new List<string>();

			int currentIndex = 0;

			foreach(object [] messageRow in messagesListStore)
			{
				messages.Add(messageRow[0].ToString());
				currentIndex ++;
			}

			xmlUtilities.SaveMessagesXMLFile (ContactTextEntry.Text, messages);
		}

		private void LoadXMLFile()
		{
			xmlUtilities.LoadMessagesXMLFile();
			ContactTextEntry.Text = xmlUtilities.ContactText;

			LoadXMLToListStore (xmlUtilities.StringArray);
		}

		private void LoadXMLToListStore(List<string> messageArray)
		{
			if(messageArray != null)
			{
				foreach(string message in messageArray)
				{
					messagesListStore.AppendValues(message);
				}
			}
		}

		private void UpdateItem()
		{

			TreeIter iter;
			
			TreePath[] treePath = MessagesTreeView.Selection.GetSelectedRows();
			
			for (int i  = treePath.Length; i > 0; i--)
			{
				messagesListStore.GetIter(out iter, treePath[(i - 1)]);
				messagesListStore.SetValues(iter, MessageTextEntry.Text);
			}
		}

		private void AddItem()
		{
			messagesListStore.AppendValues(MessageTextEntry.Text);
		}

		private void DeleteItems()
		{
			TreeIter iter;
			
			TreePath[] treePath = MessagesTreeView.Selection.GetSelectedRows();
			
			for (int i  = treePath.Length; i > 0; i--)
			{
				messagesListStore.GetIter(out iter, treePath[(i - 1)]);
				string value = (string)messagesListStore.GetValue(iter, 0);
				messagesListStore.Remove(ref iter);
			}
		}

		// Events

		public void AddButton_Clicked(System.Object Obj, EventArgs args)
		{
			string messageToAdd = MessageTextEntry.Text;

			if(messageToAdd.Trim () != "")
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

