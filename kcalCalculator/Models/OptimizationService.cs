using System;
using System.Collections.Generic;
using System.Text;
using Google.OrTools.LinearSolver;

namespace kcalCalculator.Models
{
    public class OptimizationService
    {
        public string CalculateOptimalBasket(IEnumerable<BaseProduct> products, ConstraintsContext constraints)
        {
            try
            {
                // ==========================================
                // 1. БЛОК ВАЛІДАЦІЇ ДАНИХ (Перевірка на мінуси)
                // ==========================================
                
                // Перевіряємо вимоги (праву панель)
                if (constraints.MaxBudget < 0 || constraints.MinProteins < 0 || constraints.MaxProteins < 0 || 
                    constraints.MinFats < 0 || constraints.MaxFats < 0 || constraints.MinCarbs < 0 || constraints.MaxCarbs < 0)
                {
                    return "❌ ПОМИЛКА: Вимоги до кошика не можуть бути від'ємними!";
                }

                // Перевіряємо продукти (таблицю)
                foreach (var p in products)
                {
                    if (p.Price < 0 || p.Calories < 0 || p.Proteins < 0 || p.Fats < 0 || p.Carbs < 0 || p.MinQuantity < 0 || p.MaxQuantity < 0)
                    {
                        return $"❌ ПОМИЛКА: Продукт '{p.Name}' містить від'ємні значення! Вартість, вага та БЖВ не можуть бути меншими за нуль.";
                    }
                    
                    // Бонус: перевірка, щоб мінімум не був більшим за максимум
                    if (p.MinQuantity > p.MaxQuantity)
                    {
                        return $"❌ ПОМИЛКА: У продукту '{p.Name}' мінімальна кількість більша за максимальну!";
                    }
                }

                // ==========================================
                // 2. БЛОК РОЗРАХУНКУ (Лінійне програмування)
                // ==========================================
                
                Solver solver = Solver.CreateSolver("GLOP");
                if (solver == null) return "Помилка ініціалізації розв'язувача.";

                var variables = new Dictionary<BaseProduct, Variable>();
                Objective objective = solver.Objective();
                objective.SetMinimization();

                // ... ТУТ ЙДЕ ВЕСЬ ВАШ ПОПЕРЕДНІЙ КОД СТВОРЕННЯ ЗМІННИХ ...
                foreach (var product in products)
                {
                    Variable x = solver.MakeNumVar(product.MinQuantity, product.MaxQuantity, product.Name);
                    variables.Add(product, x);
                    objective.SetCoefficient(x, product.Calories);
                }

                Constraint budgetConstraint = solver.MakeConstraint(0, constraints.MaxBudget, "Budget");
                foreach (var product in products)
                {
                    budgetConstraint.SetCoefficient(variables[product], product.Price);
                }

                Constraint proteinConstraint = solver.MakeConstraint(constraints.MinProteins, constraints.MaxProteins, "Proteins");
                Constraint fatConstraint = solver.MakeConstraint(constraints.MinFats, constraints.MaxFats, "Fats");
                Constraint carbConstraint = solver.MakeConstraint(constraints.MinCarbs, constraints.MaxCarbs, "Carbs");

                foreach (var product in products)
                {
                    proteinConstraint.SetCoefficient(variables[product], product.Proteins);
                    fatConstraint.SetCoefficient(variables[product], product.Fats);
                    carbConstraint.SetCoefficient(variables[product], product.Carbs);
                }

                Solver.ResultStatus resultStatus = solver.Solve();

                // ... ТУТ ЙДЕ ВАШ ОНОВЛЕНИЙ БЛОК ВИВОДУ РЕЗУЛЬТАТУ ...
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
                        if (quantity > 0.01)
                        {
                            totalCost += quantity * product.Price;
                            totalCalories += quantity * product.Calories;
                            totalProteins += quantity * product.Proteins;
                            totalFats += quantity * product.Fats;
                            totalCarbs += quantity * product.Carbs;

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
                return $"Критична помилка розрахунку: {ex.Message}";
            }
        }
    }
}