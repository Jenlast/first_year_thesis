namespace kcalCalculator.Models
{

    // <summary>
    // Дочірній клас для продуктів, що вимірюються поштучно (яйця, хліб тощо).
    // </summary>
    public class UnitProduct : BaseProduct
    {

        // <summary>
        // Перевизначений метод отримання типу продукту.
        // </summary>
        public override string GetProductType() => "Поштучний";

        // <summary>
        // Перевизначена властивість одиниці виміру. Для поштучних продуктів це 1 шт.
        // </summary>
        public override string MeasurementType => "1 шт";

        public override double DisplayMinQuantity 
        { 
            get => MinQuantity; 
            set => MinQuantity = value; 
        }

        public override double DisplayMaxQuantity 
        { 
            get => MaxQuantity; 
            set => MaxQuantity = value; 
        }

        public override string UnitName => "шт";
    }
}