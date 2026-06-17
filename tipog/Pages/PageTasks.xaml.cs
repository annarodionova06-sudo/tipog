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
using tipog.ApplicationData;

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

            // Заполняем ComboBox фильтра
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

            // Загружаем продукцию и обновляем счетчик
            FindProduct();
            UpdateCartCount();
        }

        // Метод обновления счетчика корзины
        private void UpdateCartCount()
        {
            try
            {
                // Считаем общее количество товаров в корзине
                var cartCount = AppConnect.Model1.orders.Where(c => c.id_user == AppConnect.id_user).Sum(c => (int?)c.quantity) ?? 0;

                // Обновляем ToolTip
                if (cartCount > 0)
                {
                    ttCartCount.Content = $"{cartCount} товаров в корзине";
                    btnOrder.ToolTip = ttCartCount;
                }
                else
                {
                    ttCartCount.Content = "Корзина пуста";
                    btnOrder.ToolTip = ttCartCount;
                }
            }
            catch
            {
                ttCartCount.Content = "Ошибка подсчета";
            }
        }

        private products[] FindProduct()
        {
            var product = AppConnect.Model1.products.ToList();
            var productall = product;

            // Поиск по названию
            if (!string.IsNullOrWhiteSpace(TextSearch.Text))
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

            // Обновляем счетчик
            if (product.Count > 0)
            {
                tbCounter.Text = "Найдено " + product.Count + " из " + productall.Count;
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

        // Кнопка ДОБАВИТЬ (новая продукция)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageEdit(null));
        }

        // Кнопка КОРЗИНА (переход на страницу корзины)
        private void ButtonCartPage_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageOrder());
        }


        // Кнопка КОРЗИНА у товара (добавить в корзину)
        private void ButtonCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button.Tag as products;

            if (selectedProduct != null)
            {

                var existingCart = AppConnect.Model1.orders.FirstOrDefault(c => c.id_product == selectedProduct.id_product && c.id_user == AppConnect.id_user);

                if (existingCart != null)
                {
                    existingCart.quantity += 1;
                    MessageBox.Show(
                        $"✅ \"{selectedProduct.name}\"\n\nКоличество в корзине: {existingCart.quantity}",
                        "Добавлено в корзину",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    var cartItem = new orders
                    {
                        id_product = selectedProduct.id_product,
                        id_user = AppConnect.id_user,
                        quantity = 1,
                        price = selectedProduct.price
                    };

                    AppConnect.Model1.orders.Add(cartItem);
                    AppConnect.Model1.SaveChanges();
                    MessageBox.Show(
                        $"✅ \"{selectedProduct.name}\"\n\nТовар добавлен в корзину!",
                        "Добавлено",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                // Обновляем счетчик!
                UpdateCartCount();
            }
        }

        private void ListProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListProducts.SelectedItem is products selectedProduct)
            {
                AppDate.AppFrame.FrameMain.Navigate(new PageEdit(selectedProduct));
            }
            else
            {
                MessageBox.Show("Выделите продукцию!");
            }
        }

        private void ListProducts_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageOrder());
        }
        

        private void ButtonHelper_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new Pages.PageGPT());
        }
    }
}
