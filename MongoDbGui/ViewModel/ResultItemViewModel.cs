using ICSharpCode.TreeView;
using MongoDB.Bson;
using System.Collections.ObjectModel;
using System.Linq;

namespace MongoDbGui.ViewModel
{
    public class ResultItemViewModel : SharpTreeNode
    {
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

        public string Type { get; set; }
        
        public ResultItemViewModel(BsonElement element)
        {
            LazyLoading = true;
            Element = element;
            Type = element.Value.BsonType.ToString();
            if (element.Value.IsBsonArray)
                Value = string.Format("{0} ({1} items)", element.Value.BsonType.ToString(), element.Value.AsBsonArray.Count);
            else if (element.Value.IsBsonDocument)
                Value = string.Format("{0} ({1} fields)", element.Value.BsonType.ToString(), element.Value.AsBsonDocument.ElementCount);
            else
            {
                Value = element.Value.ToString().Replace("\n", " ").Replace("\r", " ").Replace("\\n", " ").Replace("\\r", " ");
                if (Value.Length > 100)
                    Value = Value.Substring(0, 100) + "...";
            }
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
    }
}