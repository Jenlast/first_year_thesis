namespace kcalCalculator.Models
{
    public class UnitProduct : BaseProduct
    {
        public double WeightPerUnit { get; set; } // Вага однієї штуки
        public override string GetProductType() => "Поштучний";
        public override string MeasurementType => "1 шт";
    }
}