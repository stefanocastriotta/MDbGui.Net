using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MongoDbGui.Views.Controls
{
    /// <summary>
    /// Interaction logic for ResultItem.xaml
    /// </summary>
    public partial class ResultItem : UserControl
    {
        public ResultItem()
        {
            InitializeComponent();
            //http://community.sharpdevelop.net/forums/t/11977.aspx
            //txtArea.TextView.LineTransformers.Insert(0, new HighlightingColorizer(HighlightingManager.Instance.GetDefinition("Bson")));
            //txtArea.ReadOnlySectionProvider = ReadOnlySectionDocument.Instance;
        }

        //class ReadOnlySectionDocument : IReadOnlySectionProvider
        //{
        //    public static readonly ReadOnlySectionDocument Instance = new ReadOnlySectionDocument();

        //    public bool CanInsert(int offset)
        //    {
        //        return false;
        //    }

        //    public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
        //    {
        //        return Enumerable.Empty<ISegment>();
        //    }
        //}
    }
}
