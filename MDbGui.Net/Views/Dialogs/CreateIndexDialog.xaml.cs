using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
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
using System.Windows.Shapes;

namespace MDbGui.Net.Views.Dialogs
{
    /// <summary>
    /// Logica di interazione per CreateIndexView.xaml
    /// </summary>
    public partial class CreateIndexDialog : Window
    {
        public CreateIndexDialog()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<CreateIndexViewModel>>(this, (message) => NotificationMessageHandler(message));
        }

        private void NotificationMessageHandler(NotificationMessage<CreateIndexViewModel> message)
        {
            if (message.Notification == Constants.CreateIndexMessage || message.Notification == Constants.RecreateIndexMessage)
            {
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
