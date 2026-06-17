using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace tipog.ApplicationData
{
    /// <summary>
    /// 🧙 ПРОСТЕЙШИЙ КЛИЕНТ YANDEXGPT - ВСЁ В ОДНОМ ФАЙЛЕ!
    /// Адаптирован под типографию / печать сувенирной продукции
    /// </summary>
    public static class YandexGPT
    {
        // ==========================================
        // 🔧 НАСТРОЙКИ - ЗАПОЛНИ СВОИМИ ДАННЫМИ!
        // ==========================================

        // 1️⃣ ВСТАВЬ СВОЙ FOLDER_ID (из консоли Yandex Cloud)
        public static string FolderId { get; set; } = "b1gtttcfflfcpukaiej2";

        // 2️⃣ ВСТАВЬ СВОЙ API-КЛЮЧ (из сервисного аккаунта)
        public static string ApiKey { get; set; } = "AQVN3GfraqlULm6z8mpFvEXwqPzn490084SVidgQ";


        private static string SystemPrompt =
            "Ты — помощник типографии по печати сувенирной продукции. " +
            "Отвечай на русском, коротко, по делу. " +
            "Помогай с выбором продукции, материалов, дизайном.";

        // 📊 Статистика
        public static int TotalRequests { get; private set; } = 0;
        public static DateTime LastRequestTime { get; private set; }

        /// <summary>
        /// ✅ ПРОВЕРКА ПОДКЛЮЧЕНИЯ
        /// </summary>
        public static async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await SendMessageAsync("Привет! Напиши 'OK'");
                return response.Contains("OK") || !string.IsNullOrEmpty(response);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 💬 ОТПРАВКА СООБЩЕНИЯ - САМЫЙ ВАЖНЫЙ МЕТОД!
        /// </summary>
        public static async Task<string> SendMessageAsync(string userMessage)
        {
            // ❗ Проверка настроек
            if (string.IsNullOrWhiteSpace(FolderId) || string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new Exception("❌ Не указан FolderId или ApiKey в файле YandexGPT.cs!");
            }

            try
            {
                TotalRequests++;
                LastRequestTime = DateTime.Now;

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // 📦 Формируем запрос
                    var request = new
                    {
                        modelUri = $"gpt://{FolderId}/yandexgpt/latest",
                        completionOptions = new
                        {
                            stream = false,
                            temperature = 0.6,
                            maxTokens = 1000
                        },
                        messages = new[]
                        {
                            new { role = "system", text = SystemPrompt },
                            new { role = "user", text = userMessage }
                        }
                    };

                    // 📤 Отправляем
                    string json = JsonSerializer.Serialize(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Api-Key {ApiKey}");

                    var response = await client.PostAsync(
                        "https://llm.api.cloud.yandex.net/foundationModels/v1/completion",
                        content);

                    var responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Ошибка API: {response.StatusCode}");
                    }

                    // 📥 Парсим ответ
                    using (var doc = JsonDocument.Parse(responseJson))
                    {
                        var result = doc.RootElement
                            .GetProperty("result")
                            .GetProperty("alternatives")[0]
                            .GetProperty("message")
                            .GetProperty("text")
                            .GetString();

                        return result ?? "...";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"🧙 Ошибка YandexGPT: {ex.Message}");
            }
        }

        // ==========================================
        // 🖨️ МЕТОДЫ ДЛЯ ТИПОГРАФИИ
        // ==========================================

        /// <summary>
        /// 🎨 Рекомендация типа продукции
        /// </summary>
        public static async Task<string> RecommendProductAsync(string customerNeeds)
        {
            return await SendMessageAsync(
                $"Клиенту нужно: {customerNeeds}. Какой тип продукции порекомендуешь? Коротко.");
        }

        /// <summary>
        /// 📋 Генерация описания продукции
        /// </summary>
        public static async Task<string> ImproveDescriptionAsync(string description)
        {
            return await SendMessageAsync(
                $"Улучши описание продукции, сделай привлекательным: \"{description}\"");
        }

        /// <summary>
        /// 💡 Советы по материалам
        /// </summary>
        public static async Task<string> AdviseMaterialAsync(string productType)
        {
            return await SendMessageAsync(
                $"Какой материал лучше для '{productType}'? Дай 2-3 варианта с пояснением.");
        }

        /// <summary>
        /// 💾 СОХРАНЕНИЕ НАСТРОЕК
        /// </summary>
        public static void SaveSettings()
        {
            var settings = new
            {
                FolderId,
                ApiKey
            };

            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText("yandex_config.json", json);
        }

        /// <summary>
        /// 📂 ЗАГРУЗКА НАСТРОЕК
        /// </summary>
        public static void LoadSettings()
        {
            try
            {
                if (File.Exists("yandex_config.json"))
                {
                    string json = File.ReadAllText("yandex_config.json");
                    using (var doc = JsonDocument.Parse(json))
                    {
                        if (doc.RootElement.TryGetProperty("FolderId", out var fId))
                            FolderId = fId.GetString() ?? "";

                        if (doc.RootElement.TryGetProperty("ApiKey", out var aKey))
                            ApiKey = aKey.GetString() ?? "";
                    }
                }
                else
                {
                    // Создаём файл-образец
                    SaveSettings();
                }
            }
            catch { /* Игнорим ошибки загрузки */ }
        }
    }

    /// <summary>
    /// 🧙 РАСШИРЕНИЯ ДЛЯ УДОБСТВА
    /// </summary>
    public static class YandexGPTExtensions
    {
        public static async Task<string> AskYandexGPT(this string question)
        {
            return await YandexGPT.SendMessageAsync(question);
        }
    }
}