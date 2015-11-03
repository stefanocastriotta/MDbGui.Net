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
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class AggregateView : UserControl
    {
        public AggregateView()
        {
            InitializeComponent();
            pipelineEditor.Options.EnableHyperlinks = false;
            pipelineEditor.Options.EnableEmailHyperlinks = false;
        }
    }
}
