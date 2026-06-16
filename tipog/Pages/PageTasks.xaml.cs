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
    /// Логика взаимодействия для PageTasks.xaml
    /// </summary>
    public partial class PageTasks : Page
    {
        public PageTasks()
        {
            InitializeComponent();

            ComboFilter.Items.Add("Все типы");
            var types = AppConnect.Model1.type_products.ToList();
            foreach (var type in types)
            {
                ComboFilter.Items.Add(type.name);
            }
            ComboFilter.SelectedIndex = 0;

            // Заполняем ComboBox сортировки
            ComboSort.Items.Add("Без сортировки");
            ComboSort.Items.Add("Цена: по возрастанию");
            ComboSort.Items.Add("Цена: по убыванию");
            ComboSort.SelectedIndex = 0;

            // Загружаем продукцию
            FindProduct();

            ListProducts.ItemsSource = AppConnect.Model1.products.ToList();
            List<products> products = AppConnect.Model1.products.ToList();
            if (products.Count > 0)
            {
                tbCounter.Text = "Найдено " + products.Count + " продукции";
            }
            else
            {
                tbCounter.Text = "Не найдено";
            }
            ListProducts.ItemsSource = products;
        }
        private products[] FindProduct()
        {
            var product = AppConnect.Model1.products.ToList();
            var productall = product;

            // Поиск по названию
            if (TextSearch != null && !string.IsNullOrEmpty(TextSearch.Text))
            {
                product = product.Where(x => x.name.ToLower().Contains(TextSearch.Text.ToLower())).ToList();
            }

            // Фильтрация по типу продукции
            if (ComboFilter.SelectedIndex > 0)
            {
                string selectedType = ComboFilter.SelectedItem.ToString();
                product = product.Where(x => x.type_products.name == selectedType).ToList();
            }

            // Сортировка по цене
            if (ComboSort.SelectedIndex > 0)
            {
                switch (ComboSort.SelectedIndex)
                {
                    case 1:
                        product = product.OrderBy(x => x.price).ToList();
                        break;
                    case 2:
                        product = product.OrderByDescending(x => x.price).ToList();
                        break;
                }
            }

            if (product.Count > 0)
            {
                tbCounter.Text = "Найдено " + product.Count + " из " + productall.Count + " продукции";
            }
            else
            {
                tbCounter.Text = "Не найдено";
            }

            ListProducts.ItemsSource = product;
            return product.ToArray();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FindProduct();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FindProduct();
        }

        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FindProduct();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            products selectProduct = ListProducts.SelectedItem as products;
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageEdit(selectProduct));
        }
        private void ListProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListProducts.SelectedItems is products selectedProducts)
            {
                NavigationService.Navigate(new PageEdit(selectedProducts));
                ListProducts.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выделите продукцию!");
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonFavorite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonCart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageLike());
        }
    }
}
