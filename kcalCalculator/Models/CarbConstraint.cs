using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace kcalCalculator.Models
{

public class CarbConstraint : BaseAppConstraint
    {
        public CarbConstraint(double min, double max) : base(min * 7, max * 7, "Carbs") { }
        protected override double GetCoefficient(BaseProduct product) => product.Carbs;
    }
}