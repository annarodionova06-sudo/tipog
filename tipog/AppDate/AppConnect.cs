using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tipog.ApplicationData;

namespace tipog.AppDate
{
    internal class AppConnect
    {
        public static tipografEntities2 Model1;
        public static int id_user;
        /// <summary>
        /// 🚀 ИНИЦИАЛИЗАЦИЯ YANDEXGPT
        /// </summary>
        public static async Task InitYandexGPTAsync()
        {
            // Загружаем настройки 
            ApplicationData.YandexGPT.LoadSettings();
            // Проверяем подключение 
            if (await ApplicationData.YandexGPT.TestConnectionAsync())
            {
                System.Diagnostics.Debug.WriteLine("✅ YandexGPT готов!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ YandexGPT не настроен");
            }
        }
    }


}
