﻿using GalaSoft.MvvmLight.CommandWpf;
using ICSharpCode.TreeView;
using MDbGui.Net.Utils;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MDbGui.Net.ViewModel
{
    public class ResultItemViewModel : SharpTreeNode
    {
        JsonWriterSettingsExtended jsonWriterSettings = new JsonWriterSettingsExtended() { Indent = true, UseLocalTime = true };

        private BsonElement Element { get; set; }

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

        public override bool ShowExpander
        {
            get
            {
                return Element.Value.IsBsonArray || Element.Value.IsBsonDocument;
            }
        }

        public override bool IsCheckable
        {
            get
            {
                return false;
            }
        }

        public override object Text
        {
            get
            {
                return Element.Name;
            }
        }

        public string Id { get; set; }

        public string Value { get; set; }

        public string _toolTip { get; set; }
        public override object ToolTip
        {
            get
            {
                return _toolTip;
            }
        }

        public string Type { get; set; }

        public RelayCommand CopyToClipboard { get; set; }

        public RelayCommand CopyName { get; set; }

        public RelayCommand CopyValue { get; set; }

        public ResultItemViewModel(BsonElement element)
        {
            LazyLoading = true;
            Element = element;
            Type = element.Value.BsonType.ToString();
            if (element.Value.IsBsonArray)
            {
                Value = _toolTip = string.Format("{0} ({1} items)", element.Value.BsonType.ToString(), element.Value.AsBsonArray.Count);
            }
            else if (element.Value.IsBsonDocument)
                Value = _toolTip = string.Format("{0} ({1} fields)", element.Value.BsonType.ToString(), element.Value.AsBsonDocument.ElementCount);
            else if (element.Value.IsValidDateTime)
            {
                Value = _toolTip = element.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.FFFzzz");
            }
            else
            {
                Value = _toolTip = element.Value.ToJson(jsonWriterSettings).Replace("\n", " ").Replace("\r", " ").Replace("\\n", " ").Replace("\\r", " ").Trim(' ', '\"');
                if (Value.Length > 100)
                    Value = Value.Substring(0, 100) + "...";
                if (_toolTip.Length > 1000)
                    _toolTip = _toolTip.Substring(0, 1000) + "...";
            }

            CopyToClipboard = new RelayCommand(() =>
            {
                string res = "";
                if (Element.Value.IsBsonDocument)
                {
                    res = Element.Value.ToJson(jsonWriterSettings);
                }
                else
                {
                    BsonDocument document = new BsonDocument();
                    document.Add(Element);
                    res = document.ToJson(jsonWriterSettings);
                }
                Clipboard.SetText(res);
            });
            CopyName = new RelayCommand(() =>
            {
                Clipboard.SetText(Element.Name);
            });
            CopyValue = new RelayCommand(() =>
            {
                string res = Element.Value.ToJson(jsonWriterSettings);
                Clipboard.SetText(res);
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
                if (Element.Value.IsBsonArray)
                {
                    foreach (var child in Element.Value.AsBsonArray)
                    {
                        ResultItemViewModel item = new ResultItemViewModel(new BsonElement(Element.Value.AsBsonArray.IndexOf(child).ToString(), child));
                        Children.Add(item);
                    }
                }
                else if (Element.Value.IsBsonDocument)
                {
                    foreach (var child in Element.Value.AsBsonDocument)
                    {
                        ResultItemViewModel item = new ResultItemViewModel(child);
                        Children.Add(item);
                    }
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
            menu.Items.Add(new MenuItem() { Header = "Copy name", Command = CopyName });
            menu.Items.Add(new MenuItem() { Header = "Copy value", Command = CopyValue });
            menu.PlacementTarget = (UIElement)e.OriginalSource;
            menu.IsOpen = true;
        }
    }
}