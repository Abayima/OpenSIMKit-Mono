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

		private TreeViewColumn MessagesColumn = new TreeViewColumn();
		private ListStore MessagesListStore = new ListStore(typeof(string));
		private CellRendererText MessageCellText = new CellRendererText();

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
			List<string> Messages = new List<string>();

			int CurrentIndex = 0;

			foreach(object [] MessageRow in MessagesListStore)
			{
				Messages.Add(MessageRow[0].ToString());
				CurrentIndex ++;
			}

			xmlUtilities.SaveMessagesXMLFile (ContactTextEntry.Text, Messages);
		}

		private void LoadXMLFile()
		{
			xmlUtilities.LoadMessagesXMLFile();
			ContactTextEntry.Text = xmlUtilities.TheContactText;

			LoadXMLToListStore (xmlUtilities.TheStringArray);
		}

		private void LoadXMLToListStore(string [] MessageArray)
		{
			if(MessageArray != null)
			{
				foreach(string Message in MessageArray)
				{
					MessagesListStore.AppendValues(Message);
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

