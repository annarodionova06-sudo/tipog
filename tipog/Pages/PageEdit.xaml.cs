using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для PageEdit.xaml
    /// </summary>
    public partial class PageEdit : Page
    {
        public products product = new products();
        public PageEdit(products product1)
        {
            InitializeComponent();
            Fill();
            if (product1 != null)
            {
                product = product1;
            }
            DataContext = product;
        }
        public void Fill()
        {
            var typeProd = AppConnect.Model1.type_products;
            cmbTypeProd.Items.Add("Тип продукции");
            foreach (var item in typeProd)
            {
                cmbTypeProd.Items.Add(item.name);
            }
            cmbTypeProd.SelectedIndex = 0;

            var material = AppConnect.Model1.materials;
            cmbMaterial.Items.Add("Материал");
            foreach (var item in material)
            {
                cmbMaterial.Items.Add(item.name);
            }
            cmbMaterial.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            {
                if (product.id_product == 0)
                {
                    MessageBox.Show("Нечего удалять! Запись еще не сохранена.");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить продукцию \"" 
                    + product.name + "\"?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        AppConnect.Model1.products.Remove(product);
                        AppConnect.Model1.SaveChanges();

                        MessageBox.Show("Продукция удалена!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            AppFrame.FrameMain.Navigate(new PageTasks());
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if(string.IsNullOrWhiteSpace(product.name))
            {
                sb.AppendLine("Добавьте название!\n");
            }

            if (cmbTypeProd.SelectedIndex == 0)
            {
                sb.AppendLine("Выберите тип продукции!\n");
            }
            if (cmbMaterial.SelectedIndex == 0)
            {
                sb.AppendLine("Выберите материал!\n");
            }
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString());
                return;
            }
            else
            {
                string messag = "Изменения сохранены!";
                if(product.id_product == 0)
                {
                    product.id_type_products = AppDate.AppConnect.Model1.type_products.FirstOrDefault(x => x.name == cmbTypeProd.Text).id_type_product;
                    product.id_materials = AppDate.AppConnect.Model1.materials.FirstOrDefault(x => x.name == cmbMaterial.Text).id_material;
                
                    AppConnect.Model1.products.Add(product);
                    messag = "Запись добавлена";
                }
                try
                {
                    AppConnect.Model1.SaveChanges();
                    MessageBox.Show(messag);
                }
                catch(Exception ex)  
                {
                    MessageBox.Show(ex.Message);
                }
            }
            AppFrame.FrameMain.Navigate(new PageTasks());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
            if (ofd.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(ofd.FileName);
                string newPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                System.IO.Directory.CreateDirectory(
                    System.IO.Path.GetDirectoryName(newPath));
                System.IO.File.Copy(ofd.FileName, newPath, true);

                product.image = "/Images/" + fileName;
                txtImagePath.Text = fileName;

                imgProduct.Source = new System.Windows.Media.Imaging.BitmapImage(
               new Uri(AppDomain.CurrentDomain.BaseDirectory + product.image, UriKind.Absolute));

                MessageBox.Show("Изображение загружено!");
            }
        }
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void btnA_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.GoBack();
        }
    }
}
