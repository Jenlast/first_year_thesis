using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace kcalCalculator.Models.Constraints
{

public class ProteinConstraint : AppConstraints
    {
        // БЖВ множаться на 7 (перевід денної норми в тижневу)
        public ProteinConstraint(double min, double max) : base(min * 7, max * 7, "Proteins") { }
        protected override double GetCoefficient(BaseProduct product) => product.Proteins;
    }
}