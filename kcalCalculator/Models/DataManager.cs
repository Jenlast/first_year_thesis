using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace kcalCalculator.Models
{
    public class DataManager
    {
        private readonly string _filePath = "products_db.json";

        public void SaveProducts(List<WeightProduct> products)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(products, options);
            File.WriteAllText(_filePath, json);
        }

        public List<WeightProduct> LoadProducts()
        {
            if (!File.Exists(_filePath)) return new List<WeightProduct>();

            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<WeightProduct>>(json) ?? new List<WeightProduct>();
        }
    }
}