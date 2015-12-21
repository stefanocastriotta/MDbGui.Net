using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Views.Controls
{
    public class TextEditorExt : TextEditor
    {
        public TextEditorExt() : base()
        {
            Options.EnableHyperlinks = false;
            Options.EnableEmailHyperlinks = false;
            SearchPanel.Install(this);
        }
    }
}
