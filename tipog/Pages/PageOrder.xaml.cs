using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using tipog.AppDate;

namespace tipog.Pages
{
    public partial class PageOrder : Page
    {

        List<orders> orderItems;

        public PageOrder()
        {
            InitializeComponent();
            UpdateCart();
        }

        private void UpdateCart()
        {
            orderItems = AppConnect.Model1.orders.Where(c => c.id_user == AppConnect.id_user).ToList();
            ListCart.ItemsSource = orderItems;

            decimal total = 0;
            foreach (var item in orderItems)
            {
                var product = AppConnect.Model1.products
                    .FirstOrDefault(p => p.id_product == item.id_product);

                if (product != null)
                {
                    decimal price = product.price ?? 0;
                    int qty = item.quantity ?? 0;
                    total += price * qty;
                }
            }

            tbTotal.Text = "Итого: " + total.ToString("0.00") + " руб.";
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button.Tag as orders;

            if (cartItem != null)
            {
                cartItem.quantity = (cartItem.quantity ?? 0) + 1;
                AppConnect.Model1.SaveChanges();
                UpdateCart();
            }
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button.Tag as orders;

            if (cartItem != null && (cartItem.quantity ?? 0) > 1)
            {
                cartItem.quantity = cartItem.quantity - 1;
                AppConnect.Model1.SaveChanges();
                UpdateCart();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Удалить товар из корзины?",
                "Удаление",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var button = sender as Button;
                var cartItem = button.Tag as orders;

                if (cartItem != null)
                {
                    AppConnect.Model1.orders.Remove(cartItem);
                    AppConnect.Model1.SaveChanges();
                    UpdateCart();
                    MessageBox.Show("Товар удален из корзины!");
                }
            }
        }

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {
            if (orderItems == null || orderItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            var result = MessageBox.Show(
                "Оформить заказ на сумму " + tbTotal.Text + "?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int totalCount = 0;
                    foreach (var item in orderItems)
                    {
                        // ИСПРАВЛЕНО: добавили ?? 0
                        totalCount += (item.quantity ?? 0);
                    }

                    var newOrder = new orders
                    {
                        order_number = "ORD-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                        id_order_status = 1,
                        id_clients = 1,
                        order_date = DateTime.Now,
                        count = totalCount.ToString(),
                        id_employees = null
                    };

                    AppConnect.Model1.orders.Add(newOrder);
                    AppConnect.Model1.SaveChanges();

                    foreach (var item in orderItems)
                    {
                        var product = AppConnect.Model1.products
                            .FirstOrDefault(p => p.id_product == item.id_product);

                        if (product != null)
                        {
                            var orderStructure = new order_structures
                            {
                                id_orders = newOrder.id_order,
                                id_products = item.id_product,
                                quantity = item.quantity,
                                price = product.price
                            };
                            AppConnect.Model1.order_structures.Add(orderStructure);
                        }
                    }

                    AppConnect.Model1.SaveChanges();

                    foreach (var item in orderItems.ToList())
                    {
                        AppConnect.Model1.orders.Remove(item);
                    }
                    AppConnect.Model1.SaveChanges();

                    MessageBox.Show("Заказ №" + newOrder.order_number + " оформлен!");
                    UpdateCart();
                    AppFrame.FrameMain.Navigate(new PageQRCode(newOrder));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new PageTasks());
        }

        private void ButtonPrintReceipt_Click(object sender, RoutedEventArgs e)
        {
            if (orderItems == null || orderItems.Count == 0)
            {
                MessageBox.Show("Нет заказов для формирования чека!");
                return;
            }

            // Диалог сохранения
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                FileName = $"Чек_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    CreatePDFReceipt(saveFileDialog.FileName);
                    MessageBox.Show("Чек успешно создан!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании чека: " + ex.Message);
                }
            }
        }

        private void CreatePDFReceipt(string filePath)
        {
            // Создаем PDF документ
            Document doc = new Document(PageSize.A4, 40, 40, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

            doc.Open();

            // Заголовок
            Font titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            Paragraph title = new Paragraph("ЧЕК О ЗАКАЗЕ", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            doc.Add(new Paragraph("\n"));

            // Информация о заказе
            var lastOrder = orderItems.OrderByDescending(o => o.id_order).FirstOrDefault();

            Font infoFont = FontFactory.GetFont("Arial", 12);

            doc.Add(new Paragraph($"Номер заказа: {lastOrder.order_number}", infoFont));
            doc.Add(new Paragraph($"Дата: {lastOrder.order_date}", infoFont));
            doc.Add(new Paragraph("\n"));

            // Таблица с товарами
            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 40, 20, 20, 20 });

            // Заголовки таблицы
            table.AddCell("Наименование");
            table.AddCell("Количество");
            table.AddCell("Цена");
            table.AddCell("Сумма");

            // Товары
            decimal total = 0;
            foreach (var item in orderItems)
            {
                var product = AppConnect.Model1.products
                    .FirstOrDefault(p => p.id_product == item.id_product);

                if (product != null)
                {
                    decimal price = product.price ?? 0;
                    int qty = item.quantity ?? 0;
                    decimal sum = price * qty;
                    total += sum;

                    table.AddCell(product.name);
                    table.AddCell(qty.ToString());
                    table.AddCell(price.ToString("0.00") + " руб.");
                    table.AddCell(sum.ToString("0.00") + " руб.");
                }
            }

            doc.Add(table);
            doc.Add(new Paragraph("\n"));

            // Итого
            Font totalFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            Paragraph totalParagraph = new Paragraph($"ИТОГО: {total.ToString("0.00")} руб.", totalFont);
            totalParagraph.Alignment = Element.ALIGN_RIGHT;
            doc.Add(totalParagraph);

            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("Спасибо за заказ!", infoFont));

            doc.Close();
        }
    }
    
}