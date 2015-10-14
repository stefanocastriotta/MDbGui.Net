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
        }

        public override bool CanCopy(SharpTreeNode[] nodes)
        {
            return false;
        }

        protected override IDataObject GetDataObject(SharpTreeNode[] nodes)
        {
            var data = new DataObject();
            var paths = nodes.Select(n => n.Text).ToArray();
            data.SetData(DataFormats.FileDrop, paths);
            return data;
        }

        public override bool CanDelete(SharpTreeNode[] nodes)
        {
            return false;
        }

        public override void Delete(SharpTreeNode[] nodes)
        {
        }

        public override void DeleteWithoutConfirmation(SharpTreeNode[] nodes)
        {
        }

        public override bool CanPaste(IDataObject data)
        {
            return data.GetDataPresent(DataFormats.FileDrop);
        }

        public override void Paste(IDataObject data)
        {
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