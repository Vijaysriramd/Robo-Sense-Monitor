using Robo_Sense_Monitor.Commands;
using Robo_Sense_Monitor.Model;
using Robo_Sense_Monitor.ViewModel;
using Robo_Sense_Monitor.Model;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Robo_Sense_Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      

        public MainWindow()
        {
            InitializeComponent();
            Uri iconUri = new Uri("pack://application:,,,/Icons/AppHeader.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            this.Title = "Robo Sense Monitor";
            MainWindowViewModel objMain = new MainWindowViewModel();
            this.DataContext = objMain;

           
        }

        

    }

}