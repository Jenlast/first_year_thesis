using System;
using System.Collections.Generic;
using System.Text;
using Google.OrTools.LinearSolver;

namespace kcalCalculator.Models
{
    public class OptimizationService
    {
        // Метод повертає рядок з результатом (або повідомленням про помилку)
        public string CalculateOptimalBasket(IEnumerable<BaseProduct> products, ConstraintsContext constraints)
        {
            try
            {
                // Створюємо солвер для лінійного програмування (GLOP)
                Solver solver = Solver.CreateSolver("GLOP");
                if (solver == null) return "Помилка ініціалізації розв'язувача.";

                // Словник для зберігання змінних (x_i - кількість кожного продукту)
                var variables = new Dictionary<BaseProduct, Variable>();

                // Цільова функція: Мінімізація калорій (Вимога 1)
                Objective objective = solver.Objective();
                objective.SetMinimization();

                // 1. Створення змінних та базових обмежень (Вимога 2а - мінімум/максимум продукту)
                foreach (var product in products)
                {
                    // Змінна x >= MinQuantity та x <= MaxQuantity
                    Variable x = solver.MakeNumVar(product.MinQuantity, product.MaxQuantity, product.Name);
                    variables.Add(product, x);

                    // Додаємо калорійність цього продукту до цільової функції: x * Calories
                    objective.SetCoefficient(x, product.Calories);
                }

                // 2. Обмеження бюджету: Сума (x_i * Price_i) <= MaxBudget (Вимога 2б)
                Constraint budgetConstraint = solver.MakeConstraint(0, constraints.MaxBudget, "Budget");
                foreach (var product in products)
                {
                    budgetConstraint.SetCoefficient(variables[product], product.Price);
                }

                // 3. Макроелементи (Білки, Жири, Вуглеводи) (Вимога 2в)
                Constraint proteinConstraint = solver.MakeConstraint(constraints.MinProteins, constraints.MaxProteins, "Proteins");
                Constraint fatConstraint = solver.MakeConstraint(constraints.MinFats, constraints.MaxFats, "Fats");
                Constraint carbConstraint = solver.MakeConstraint(constraints.MinCarbs, constraints.MaxCarbs, "Carbs");

                foreach (var product in products)
                {
                    proteinConstraint.SetCoefficient(variables[product], product.Proteins);
                    fatConstraint.SetCoefficient(variables[product], product.Fats);
                    carbConstraint.SetCoefficient(variables[product], product.Carbs);
                }

                // РОЗВ'ЯЗАННЯ
                Solver.ResultStatus resultStatus = solver.Solve();

                // Формування результату
                if (resultStatus == Solver.ResultStatus.OPTIMAL)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("УСПІХ! Знайдено оптимальний кошик:\n");
                    
                    double totalCost = 0;
                    
                    foreach (var product in products)
                    {
                        double quantity = variables[product].SolutionValue();
                        if (quantity > 0.01) // Виводимо лише те, що потрібно купити
                        {
                            sb.AppendLine($"- {product.Name}: {Math.Round(quantity, 2)} од.");
                            totalCost += quantity * product.Price;
                        }
                    }
                    
                    sb.AppendLine($"\nЗагальна калорійність: {Math.Round(solver.Objective().Value(), 2)} ккал");
                    sb.AppendLine($"Загальна вартість: {Math.Round(totalCost, 2)} грн");
                    return sb.ToString();
                }
                else
                {
                    return "Неможливо знайти рішення. Спробуйте послабити обмеження (наприклад, збільшити бюджет або змінити межі БЖВ).";
                }
            }
            catch (Exception ex)
            {
                // Вимога методички: Використання механізму виключень
                return $"Критична помилка розрахунку: {ex.Message}";
            }
        }
    }
}