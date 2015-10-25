using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.TreeView;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MongoDbGui.ViewModel
{
    public class DocumentResultViewModel : SharpTreeNode
    {
        public override object Icon
        {
            get
            {
                return null;
            }
        }

        public override object ExpandedIcon
        {
            get
            {
                return null;
            }
        }

        public override bool IsCheckable
        {
            get
            {
                return true;
            }
        }

        public override object Text
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Id))
                    return "#" + Index + " (" + Id + ")";
                else
                    return "#" + Index;
            }
        }

        public MongoDbServerViewModel Server { get; private set; }

        public string Database { get; private set; }

        public string Collection { get; private set; }

        public BsonDocument Result { get; private set; }

        public string Id { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public int Index { get; private set; }

        public RelayCommand EditResult { get; set; }

        public RelayCommand ConfirmDeleteResult { get; set; }

        public RelayCommand CopyToClipboard { get; set; }

        /// <summary>
        /// Initializes a new instance of the ResultItemViewModel class.
        /// </summary>
        public DocumentResultViewModel(BsonDocument result, MongoDbServerViewModel server, string database, string collection, int index)
        {
            LazyLoading = true;
            IsChecked = false;
            Result = result;
            Index = index;
            if (result.Contains("_id"))
                Id = result["_id"].ToString();
            Value = "(" + result.ElementCount + ") fields";
            Type = result.BsonType.ToString();
            Server = server;
            Database = database;
            Collection = collection;
            EditResult = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<DocumentResultViewModel>(this, "EditResult"));
            });
            CopyToClipboard = new RelayCommand(() =>
            {
                Clipboard.SetText(Result.ToJson(new JsonWriterSettings { Indent = true }));
            });
            ConfirmDeleteResult = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<DocumentResultViewModel>(this, "ConfirmDeleteResult"));
            });
        }

        public override string ToString()
        {
            return Text.ToString();
        }

        protected override void LoadChildren()
        {
            try
            {
                foreach (var element in Result)
                {
                    ResultItemViewModel item = new ResultItemViewModel(element);
                    item.Type = element.Value.BsonType.ToString();
                    if (element.Value.IsBsonArray)
                        item.Value = string.Format("{0} ({1} items)", element.Value.BsonType.ToString(), element.Value.AsBsonArray.Count);
                    else if (element.Value.IsBsonDocument)
                        item.Value = string.Format("{0} ({1} fields)", element.Value.BsonType.ToString(), element.Value.AsBsonDocument.ElementCount);
                    else
                    {
                        item.Value = element.Value.ToString().Replace("\n", " ").Replace("\r", " ").Replace("\\n", " ").Replace("\\r", " ");
                        if (item.Value.Length > 100)
                            item.Value = item.Value.Substring(0, 100) + "...";
                    }
                    Children.Add(item);
                }
            }
            catch
            {
            }
        }

        public override void ShowContextMenu(ContextMenuEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            menu.Items.Add(new MenuItem() { Header = "Copy to clipboard", Command = CopyToClipboard });
            menu.Items.Add(new MenuItem() { Header = "Edit", Command = EditResult });
            menu.Items.Add(new MenuItem() { Header = "Delete", Command = ConfirmDeleteResult });
            menu.PlacementTarget = (UIElement)e.OriginalSource;
            menu.IsOpen = true;
        }

        protected override void OnExpanding()
        {
            base.OnExpanding();
            Messenger.Default.Send(new NotificationMessage(this, ((ResultsViewModel)this.Parent).Owner, "ItemExpanding"));
        }
    }
}
