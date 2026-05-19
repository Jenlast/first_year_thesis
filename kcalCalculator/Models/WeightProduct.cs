namespace kcalCalculator.Models
{
    public class WeightProduct : BaseProduct
    {
        // Кількість вимірюється в грамах або кілограмах
        public override string GetProductType() => "Ваговий (на 100г)";
    }
}