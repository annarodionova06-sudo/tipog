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

namespace tipog.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAvtoris.xaml
    /// </summary>
    public partial class PageAvtoris : Page
    {
        public PageAvtoris()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userObj = AppDate.AppConnect.Model1.users.FirstOrDefault(x =>
                x.login == txtLogin.Text && x.password == txtPassword.Password);
                if (userObj == null)
                {
                    MessageBox.Show("Такого пользователя не существует", "Ошибка авторизации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Здравствуйте, " + userObj.name, "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка" + ex.Message.ToString(), "Критическая ошибка приложения", MessageBoxButton.OK);
            }
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageTasks());
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageReg());
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
