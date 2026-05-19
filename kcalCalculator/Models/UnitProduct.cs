namespace kcalCalculator.Models
{
    public class UnitProduct : BaseProduct
    {
        public double WeightPerUnit { get; set; } // Вага однієї штуки
        public override string GetProductType() => "Поштучний (1 шт)";
    }
}