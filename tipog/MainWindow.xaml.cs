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
using tipog.AppDate;

namespace tipog
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppDate.AppConnect.Model1 = new tipografEntities2();
            AppDate.AppFrame.FrameMain = FrameName1;
            FrameName1.Navigate(new Pages.PageAvtoris());

            _ = AppDate.AppConnect.InitYandexGPTAsync();
        }
    }
}
