using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace kcalCalculator.Models
{
    public class DataManager
    {
        public void SaveProducts(string filePath, IEnumerable<BaseProduct> products)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(products, options);
            File.WriteAllText(filePath, json);
        }

        public List<BaseProduct> LoadProducts(string filePath)
        {
            if (!File.Exists(filePath)) return new List<BaseProduct>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<BaseProduct>>(json) ?? new List<BaseProduct>();
        }
    }
}