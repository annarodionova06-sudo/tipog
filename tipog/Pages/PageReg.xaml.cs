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

namespace tipog.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageReg.xaml
    /// </summary>
    public partial class PageReg : Page
    {
        public PageReg()
        {
            InitializeComponent();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(AppDate.AppConnect.Model1.users.Count(x => x.login == txtLogin.Text) > 0)
                {
                    MessageBox.Show("Пользователь с таким логином уже есть!!", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if(String.IsNullOrEmpty(txtLogin.Text) || String.IsNullOrEmpty(txtName.Text) ||
                    String.IsNullOrEmpty(pswPass.Password) ||  String.IsNullOrWhiteSpace(pswPass.Password) ||
                    String.IsNullOrWhiteSpace(txtName.Text) || String.IsNullOrWhiteSpace(txtLogin.Text))
                {
                    MessageBox.Show("Не все поля заполнены!!", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                users userObj = new users()
                {
                    login = txtLogin.Text,
                    name = txtName.Text,
                    password = pswPass.Password,
                    birtDay = dB.SelectedDate.Value,
                    phone = txtPhone.Text,
                    email = txtEmail.Text
                };
                AppConnect.Model1.users.Add(userObj);
                AppConnect.Model1.SaveChanges();
                MessageBox.Show("Данные успешно добавлены!", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                AppDate.AppFrame.FrameMain.GoBack();
            }
            catch 
            {
                MessageBox.Show("Ошибка при добавлении данных!", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void pswPass1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pswPass.Password != pswPass1.Password)
            {
                btnReg.IsEnabled = false;
                pswPass1.Background = Brushes.LightCoral;
                pswPass1.Background = Brushes.Red;
            }
            else
            {
                btnReg.IsEnabled = true;
                pswPass1.Background = Brushes.LightGreen;
                pswPass1.Background = Brushes.Green;
            }
        }

        private void btnAgo_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.GoBack();
        }
    }
}
