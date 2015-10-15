using ICSharpCode.TreeView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ResultsViewModel : SharpTreeNode
    {
        List<BsonDocument> _documents;

        public ResultsViewModel(List<BsonDocument> documents)
        {
            _documents = documents;
            LazyLoading = true;
        }


        public override void Drop(DragEventArgs e, int index)
        {
        }

        protected override void LoadChildren()
        {
            try
            {
                foreach (var result in _documents)
                    Children.Add(new DocumentResultViewModel(result, _documents.IndexOf(result) + 1));
            }
            catch
            {
            }
        }


    }
}