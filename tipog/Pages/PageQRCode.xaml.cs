using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using tipog.AppDate;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;


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
            // Создаем QR-код с номером заказа
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 0
                }
            };

            // Генерируем QR-код с информацией о заказе
            string qrData = $"Заказ №{currentOrder.order_number}\nДата: {currentOrder.order_date}\nСумма: {CalculateTotal()} руб.";

            var result = writer.Write(qrData);

            // Конвертируем в BitmapImage для WPF
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
            tbOrderNumber.Text = $"Заказ №{currentOrder.order_number}";
        }

        private decimal CalculateTotal()
        {
            // Считаем сумму заказа
            var orderStructures = AppConnect.Model1.order_structures
                .Where(os => os.id_orders == currentOrder.id_order)
                .ToList();

            decimal total = 0;
            foreach (var item in orderStructures)
            {
                total += (item.price ?? 0) * (item.quantity ?? 0);
            }

            return total;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageTasks());
        }
    }
}