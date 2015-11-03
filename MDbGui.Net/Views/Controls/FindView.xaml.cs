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

namespace MDbGui.Net.Views.Controls
{
    /// <summary>
    /// Interaction logic for FindView.xaml
    /// </summary>
    public partial class FindView : UserControl
    {
        public FindView()
        {
            InitializeComponent();
            findEditor.Options.EnableHyperlinks = false;
            findEditor.Options.EnableEmailHyperlinks = false;
            sortEditor.Options.EnableHyperlinks = false;
            sortEditor.Options.EnableEmailHyperlinks = false;
            projectionEditor.Options.EnableHyperlinks = false;
            projectionEditor.Options.EnableEmailHyperlinks = false;
        }
    }
}
