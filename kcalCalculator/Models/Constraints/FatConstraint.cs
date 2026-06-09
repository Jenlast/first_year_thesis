using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace kcalCalculator.Models.Constraints
{

    public class FatConstraint : AppConstraints
    {
        public FatConstraint(double min, double max) : base(min * 7, max * 7, "Fats") { }
        protected override double GetCoefficient(BaseProduct product) => product.Fats;
    }
}