using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;

namespace MongoDbGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlReader reader = XmlReader.Create("Resources/BsonHighlighting.xml");
            HighlightingManager.Instance.RegisterHighlighting("Bson", new string[] { ".bson" }, HighlightingLoader.Load(reader, HighlightingManager.Instance));
        }
    }
}
