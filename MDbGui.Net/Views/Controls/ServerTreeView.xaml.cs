using MDbGui.Net.ViewModel;
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
    /// Interaction logic for ServerTreeView.xaml
    /// </summary>
    public partial class ServerTreeView : UserControl
    {
        public ServerTreeView()
        {
            InitializeComponent();
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid && ((TreeView)e.Source).Items.Count > 0)
            {
                foreach (var item in ((TreeView)e.Source).Items.Cast<BaseTreeviewViewModel>())
                {
                    item.IsSelected = false;
                    item.UnselectAll();
                }
                ((Grid)e.OriginalSource).Focus();
            }
        }
    }
}
