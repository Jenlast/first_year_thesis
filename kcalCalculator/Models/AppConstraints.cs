using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace kcalCalculator.Models
{
    /// <summary>
    /// Абстрактний базовий клас для всіх математичних обмежень.
    /// Демонструє поліморфізм: кожен нащадок сам вирішує, який коефіцієнт (БЖВ чи ціну) передати в алгоритм.
    /// </summary>
    public abstract class BaseAppConstraint
    {
        protected double MinBound { get; }
        protected double MaxBound { get; }
        protected string Name { get; }

        protected BaseAppConstraint(double minBound, double maxBound, string name)
        {
            MinBound = minBound;
            MaxBound = maxBound;
            Name = name;
        }

        /// <summary>
        /// Універсальний метод, який створює обмеження в Google OR-Tools та додає коефіцієнти для всіх продуктів.
        /// </summary>
        public void ApplyToSolver(Solver solver, Dictionary<BaseProduct, Variable> variables)
        {
            // Створюємо базове обмеження Google
            Constraint googleConstraint = solver.MakeConstraint(MinBound, MaxBound, Name);
            
            // Проходимо по всіх створених змінних і додаємо до них коефіцієнт
            foreach (var kvp in variables)
            {
                BaseProduct product = kvp.Key;
                Variable variable = kvp.Value;
                
                // Виклик поліморфного методу: Кожен клас-нащадок повертає свій унікальний параметр (білок, ціну тощо)
                googleConstraint.SetCoefficient(variable, GetCoefficient(product));
            }
        }

        // Абстрактний метод, який реалізують дочірні класи
        protected abstract double GetCoefficient(BaseProduct product);
    }
}