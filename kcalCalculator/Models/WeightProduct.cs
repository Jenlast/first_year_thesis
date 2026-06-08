using System.Security.Cryptography.X509Certificates;

namespace kcalCalculator.Models
{

    // <summary>
    // Дочірній клас для продуктів, що вимірюються на вагу (у грамах).
    // </summary>
    public class WeightProduct : BaseProduct
    {
        
        // <summary>
        // Перевизначений метод отримання типу продукту.
        // </summary>
        public override string GetProductType() => "Ваговий";

        // <summary>
        // Перевизначена властивість одиниці виміру. Для вагових продуктів розрахунок йде на 100 г.
        // </summary>
        public override string MeasurementType => "100 г";

        public override double DisplayMinQuantity 
        { 
            get => MinQuantity * 100; 
            set => MinQuantity = value / 100; 
        }

        public override double DisplayMaxQuantity 
        { 
            get => MaxQuantity * 100; 
            set => MaxQuantity = value / 100; 
        }

        public override string UnitName => "г";
    }
}