using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using tipog.AppDate;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;


namespace tipog.Pages
{
    public partial class PageQRCode : Page
    {
        private orders currentOrder;

        public PageQRCode(orders order)
        {
            InitializeComponent();
            currentOrder = order;
            LoadQR();
        }
        private void LoadQR()
        {
            try
            {
                // Получаем состав заказа
                var orderStructures = AppConnect.Model1.order_structures
                    .Where(os => os.id_orders == currentOrder.id_order)
                    .ToList();

                // Формируем текст состава заказа
                StringBuilder orderDetails = new StringBuilder();
                foreach (var item in orderStructures)
                {
                    var product = AppConnect.Model1.products
                        .FirstOrDefault(p => p.id_product == item.id_products);
                    if (product != null)
                    {
                        orderDetails.AppendLine($"{product.name} - {item.quantity} шт. x {product.price} руб.");
                    }
                }

                // Считаем сумму
                decimal total = 0;
                foreach (var item in orderStructures)
                {
                    var product = AppConnect.Model1.products
                        .FirstOrDefault(p => p.id_product == item.id_products);
                    if (product != null)
                    {
                        total += (product.price ?? 0) * (item.quantity ?? 0);
                    }
                }

                string baseUrl = "https://docs.google.com/forms/d/e/1FAIpQLSfqi2d1BoqxBDeitvIk4ePihaGXh4vvQBdg0FQ9F0tDhTECyg/viewform";

                // 🔧 ВАШИ ID ПОЛЕЙ из формы:
                // entry.361232479 - номер заказа
                // entry.809578365 - дата
                // entry.1573292947 - сумма
                // entry.559659275 - количество товаров
                // entry.929920653 - оценка
                // entry.266135167 - состав заказа

                var formUrl = $"{baseUrl}?" +
           $"entry.361232479={Uri.EscapeDataString(currentOrder.order_number)}&" +
           $"entry.809578365={Uri.EscapeDataString(currentOrder.order_date?.ToString("yyyy-MM-dd") ?? "")}&" +
           $"entry.559659275={Uri.EscapeDataString(orderDetails.ToString().Trim())}&" +
           $"entry.266135167={Uri.EscapeDataString(orderStructures.Sum(s => s.quantity ?? 0).ToString())}&" +
           $"entry.1573292947={Uri.EscapeDataString(total.ToString("0.00"))}";

                // Генерация QR-кода
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Width = 300,
                        Height = 300,
                        Margin = 2
                    }
                };

                var result = writer.Write(formUrl);

                // Конвертируем в BitmapImage
                var bitmap = new BitmapImage();
                using (var memoryStream = new MemoryStream())
                {
                    result.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Position = 0;
                    bitmap.BeginInit();
                    bitmap.StreamSource = memoryStream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                imgQr.Source = bitmap;
                tbOrderNumber.Text = $"Заказ №{currentOrder.order_number}\nОтсканируйте, чтобы оценить качество";

                // Для отладки - показываем URL (можно потом убрать)
                // MessageBox.Show($"URL:\n{formUrl}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации QR-кода: {ex.Message}");
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            AppDate.AppFrame.FrameMain.Navigate(new PageTasks());
        }
    }

}