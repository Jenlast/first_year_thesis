using System.Security.Cryptography.X509Certificates;

namespace kcalCalculator.Models
{
    public class WeightProduct : BaseProduct
    {
        // Кількість вимірюється в грамах або кілограмах
        public override string GetProductType() => "Ваговий";
        public override string MeasurementType => "100 г";
    }
}