using ICSharpCode.TreeView;
using MongoDB.Bson;
using System.Collections.ObjectModel;
using System.Linq;

namespace MongoDbGui.ViewModel
{
    public class ResultItemViewModel : SharpTreeNode
    {
        private BsonElement Element { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public ResultItemViewModel(BsonElement element)
        {
            Element = element;
        }
    }
}