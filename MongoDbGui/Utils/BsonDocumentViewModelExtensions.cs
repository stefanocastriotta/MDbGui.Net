using MongoDB.Bson;
using MongoDbGui.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbGui.Utils
{
    public static class BsonDocumentViewModelExtensions
    {
        public static List<ResultItemViewModel> ToResultItemViewModel(this BsonDocument document)
        {
            List<ResultItemViewModel> model = new List<ResultItemViewModel>();
            foreach (var element in document)
            {
                model.Add(GetChildren(element));
            }
            return model;
        }

        private static ResultItemViewModel GetChildren(BsonElement element)
        {
            ResultItemViewModel item = new ResultItemViewModel();
            item.Name = element.Name;
            item.Type = element.Value.BsonType.ToString();
            if (element.Value.IsBsonArray)
            {
                item.Value = string.Format("{0} ({1} items)", element.Value.BsonType.ToString(), element.Value.AsBsonArray.Count);
                foreach (var child in element.Value.AsBsonArray)
                {
                    var childrenVm = GetChildren(child);
                    childrenVm.Name = element.Value.AsBsonArray.IndexOf(child).ToString();
                    item.Children.Add(childrenVm);
                }
            }
            else if (element.Value.IsBsonDocument)
            {
                item.Value = string.Format("{0} ({1} fields)", element.Value.BsonType.ToString(), element.Value.AsBsonDocument.ElementCount);
                foreach (var child in element.Value.AsBsonDocument)
                {
                    item.Children.Add(GetChildren(child));
                }
            }
            else
                item.Value = element.Value.ToString().Replace(Environment.NewLine, " ").Replace("\r", " ");
            return item;
        }

        private static ResultItemViewModel GetChildren(BsonValue value)
        {
            ResultItemViewModel item = new ResultItemViewModel();
            item.Type = value.BsonType.ToString();
            if (value.IsBsonArray)
            {
                item.Value = string.Format("{0} ({1} items)", value.BsonType.ToString(), value.AsBsonArray.Count);
                foreach (var child in value.AsBsonArray)
                {
                    item.Children.Add(GetChildren(child));
                }
            }
            else if (value.IsBsonDocument)
            {
                item.Value = string.Format("{0} ({1} fields)", value.BsonType.ToString(), value.AsBsonDocument.ElementCount);
                foreach (var child in value.AsBsonDocument)
                {
                    item.Children.Add(GetChildren(child));
                }
            }
            else
                item.Value = value.ToString().Replace(Environment.NewLine, " ").Replace("\r", " ");
            return item;
        }
    }
}
