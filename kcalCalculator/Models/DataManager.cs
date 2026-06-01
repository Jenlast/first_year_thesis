using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace kcalCalculator.Models
{

    // <summary>
    // Клас, що відповідає за файлове введення-виведення даних.
    // Реалізує збереження та завантаження бази продуктів у форматі JSON.
    // </summary>
    public class DataManager
    {

        // <summary>
        // Зберігає список продуктів у вказаний файл JSON (поліморфна серіалізація).
        // </summary>
        // <param name="filePath">Повний шлях до файлу збереження.</param>
        // <param name="products">Колекція продуктів (BaseProduct) для збереження.</param>
        public void SaveProducts(string filePath, IEnumerable<BaseProduct> products)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(products, options);
            File.WriteAllText(filePath, json);
        }

        // <summary>
        // Завантажує список продуктів із вказаного файлу JSON.
        // </summary>
        // <param name="filePath">Повний шлях до файлу для зчитування.</param>
        // <returns>Список об'єктів-спадкоємців BaseProduct.</returns>
        public List<BaseProduct> LoadProducts(string filePath)
        {
            if (!File.Exists(filePath)) return new List<BaseProduct>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<BaseProduct>>(json) ?? new List<BaseProduct>();
        }
    }
}