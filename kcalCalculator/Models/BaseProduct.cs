using System;

namespace kcalCalculator.Models
{
    public abstract class BaseProduct
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public double Price { get; set; } // Ціна 
        public double Calories { get; set; } // Калорійність
        public double Proteins { get; set; } // Білки
        public double Fats { get; set; } // Жири
        public double Carbs { get; set; } // Вуглеводи

        public double MinQuantity { get; set; } = 0;
        public double MaxQuantity { get; set; } = 1000; 

        public abstract string GetProductType();
    }
}