using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace kcalCalculator.Models
{

public class BudgetConstraint : BaseAppConstraint
    {
        // Бюджет не множиться на 7, бо він на тиждень
        public BudgetConstraint(double maxBudget) : base(0, maxBudget, "Budget") { }
        protected override double GetCoefficient(BaseProduct product) => product.Price;
    }
}