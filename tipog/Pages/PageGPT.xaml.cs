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
using tipog.ApplicationData;

namespace tipog.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageGPT.xaml
    /// </summary>
    public partial class PageGPT : Page
    {
        public PageGPT()
        {
            InitializeComponent();
            AddMessage("🦉 Привет! Я помощник типографии. Задай вопрос о продукции, материалах или печати!", false);
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync();
        }

        private async void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await SendMessageAsync();
            }
        }

        private async System.Threading.Tasks.Task SendMessageAsync()
        {
            var message = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddMessage($"🧙 {message}", true);
            txtInput.Clear();

            AddMessage("🦉 Печатает...", false, true);

            try
            {
                var response = await YandexGPT.SendMessageAsync(message);
                RemoveTypingIndicator();
                AddMessage($"🦉 {response}", false);
            }
            catch (Exception ex)
            {
                RemoveTypingIndicator();
                AddMessage($"⚠️ Ошибка: {ex.Message}", false);
            }
        }

        private void AddMessage(string text, bool isUser, bool isTyping = false)
        {
            Dispatcher.Invoke(() =>
            {
                var border = new Border
                {
                    Background = isUser
                        ? new SolidColorBrush(Color.FromRgb(91, 155, 213))
                        : new SolidColorBrush(Color.FromRgb(245, 245, 220)),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(10),
                    Margin = new Thickness(isUser ? 50 : 10, 5, isUser ? 10 : 50, 5),
                    HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                    Name = isTyping ? "TypingIndicator" : null
                };

                var textBlock = new TextBlock
                {
                    Text = text,
                    Foreground = isUser
                        ? new SolidColorBrush(Colors.White)
                        : new SolidColorBrush(Color.FromRgb(91, 155, 213)),
                    TextWrapping = TextWrapping.Wrap
                };

                border.Child = textBlock;
                ChatPanel.Children.Add(border);
                ChatScroller.ScrollToBottom();
            });
        }

        private void RemoveTypingIndicator()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = ChatPanel.Children.Count - 1; i >= 0; i--)
                {
                    if (ChatPanel.Children[i] is Border b && b.Name == "TypingIndicator")
                    {
                        ChatPanel.Children.RemoveAt(i);
                        break;
                    }
                }
            });
        }
    }
}
        