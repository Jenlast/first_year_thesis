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
                    sb.AppendLine("🛍️ Сформований кошик:\n");
                    sb.AppendLine("Включає такі продукти:");
                    
                    double totalCost = 0;
                    double totalCalories = 0;
                    double totalProteins = 0;
                    double totalFats = 0;
                    double totalCarbs = 0;
                    
                    foreach (var product in products)
                    {
                        double quantity = variables[product].SolutionValue();
                        if (quantity > 0.01) // Якщо продукт обрано (більше нуля)
                        {
                            // Рахуємо сумарні показники
                            totalCost += quantity * product.Price;
                            totalCalories += quantity * product.Calories;
                            totalProteins += quantity * product.Proteins;
                            totalFats += quantity * product.Fats;
                            totalCarbs += quantity * product.Carbs;

                            // Логіка відображення: якщо це ваговий продукт (розрахунок на 100г),
                            // множимо на 100, щоб показати грами. Інакше - штуки.
                            double displayQuantity = quantity;
                            string unit = "од/тижд";

                            if (product is WeightProduct)
                            {
                                displayQuantity = quantity * 100;
                                unit = "г/тижд";
                            }
                            else if (product is UnitProduct)
                            {
                                unit = "шт/тижд";
                            }

                            // Додаємо рядок продукту
                            sb.AppendLine($"- {product.Name} — {Math.Round(displayQuantity, 0)} {unit}");
                        }
                    }
                    
                    sb.AppendLine("\n📊 Загальні показники на тиждень:");
                    sb.AppendLine($"- Калорій: {Math.Round(totalCalories, 1)} ккал");
                    sb.AppendLine($"- Білків: {Math.Round(totalProteins, 1)} г");
                    sb.AppendLine($"- Жирів: {Math.Round(totalFats, 1)} г");
                    sb.AppendLine($"- Вуглеводів: {Math.Round(totalCarbs, 1)} г");

                    sb.AppendLine($"\n💵 Загальна ціна: {Math.Round(totalCost, 2)} грн.");

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