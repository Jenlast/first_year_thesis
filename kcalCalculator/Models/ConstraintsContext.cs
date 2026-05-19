namespace kcalCalculator.Models
{
    public class ConstraintsContext
    {
        public double MaxBudget { get; set; } // Обмеження вартості
        public double MinProteins { get; set; }
        public double MaxProteins { get; set; }
        public double MinFats { get; set; }
        public double MaxFats { get; set; }
        public double MinCarbs { get; set; }
        public double MaxCarbs { get; set; }
    }
}