using System;
using System.Collections.Generic;
using System.Text;
using Google.OrTools.LinearSolver;
using kcalCalculator.Models.Constraints;

namespace kcalCalculator.Models
{

    // <summary>
    // Клас-сервіс, що містить математичну модель оптимізації.
    // Відповідає за перетворення об'єктів у задачу лінійного програмування та її розв'язання.
    // </summary>
    public class OptimizationService
    {

        // <summary>
        // Головний алгоритм застосунку. Валідує дані, ініціалізує лінійний розв'язувач (GLOP), 
        // задає цільову функцію мінімізації та повертає текстовий результат.
        // </summary>
        // <param name="products">База доступних продуктів.</param>
        // <param name="constraints">Вимоги користувача до бюджету та нутрієнтів.</param>
        // <returns>Текстовий звіт про сформований кошик або повідомлення про помилку.</returns>
        public string CalculateOptimalBasket(IEnumerable<BaseProduct> products, ConstraintsContext constraints)
        {
            try
            {
                // 1. Блок валідації даних (Перевірка на мінуси)
                if (constraints.MaxBudget < 0 || constraints.MinProteins < 0 || constraints.MaxProteins < 0 || 
                    constraints.MinFats < 0 || constraints.MaxFats < 0 || constraints.MinCarbs < 0 || constraints.MaxCarbs < 0)
                {
                    return "ПОМИЛКА: Вимоги до кошика не можуть бути від'ємними!";
                }

                foreach (var p in products)
                {
                    if (constraints.MaxBudget < 0 || constraints.MinProteins < 0 || constraints.MaxProteins < 0 || 
                    constraints.MinFats < 0 || constraints.MaxFats < 0 || constraints.MinCarbs < 0 || constraints.MaxCarbs < 0)
                    {
                        return $"ПОМИЛКА: Продукт '{p.Name}' містить від'ємні значення!";
                    }
                    // Визначаємо реальний мінімум: якщо галочка стоїть - беремо з таблиці, якщо ні - беремо 0
                    double actualMin = p.IsMandatory ? p.MinQuantity : 0;
                    if (actualMin > p.MaxQuantity)
                    {
                        return $"ПОМИЛКА: У продукту '{p.Name}' мінімальна кількість більша за максимальну!";
                    }
                }

                // 2. Блок розрахунку (Лінійне програмування)
                Solver solver = Solver.CreateSolver("GLOP");
                if (solver == null) return "Помилка ініціалізації розв'язувача.";

                var variables = new Dictionary<BaseProduct, Variable>();
                Objective objective = solver.Objective();
                objective.SetMinimization();

                foreach (var product in products)
                {
                    // Якщо галочка стоїть - змушуємо брати мінімум. Якщо не стоїть - дозволяємо алгоритму брати від 0
                    double actualMin = product.IsMandatory ? product.MinQuantity : 0;
                    Variable x = solver.MakeNumVar(actualMin, product.MaxQuantity, product.Name);
                    variables.Add(product, x);
                    objective.SetCoefficient(x, product.Calories); // Ціль - мінімум калорій
                }

                // Застосування обмежень (Поліморфізм у дії)
                
                // 1. Створюємо список абстрактних обмежень, наповнюючи його конкретними класами-нащадками
                var appConstraints = new List<AppConstraints>
                {
                    new BudgetConstraint((double)constraints.MaxBudget),
                    new ProteinConstraint((double)constraints.MinProteins, (double)constraints.MaxProteins),
                    new FatConstraint((double)constraints.MinFats, (double)constraints.MaxFats),
                    new CarbConstraint((double)constraints.MinCarbs, (double)constraints.MaxCarbs)
                };

                // 2. Поліморфний виклик: програма не знає, який саме це клас, вона просто викликає ApplyToSolver,
                // а кожен об'єкт сам знає, як правильно додати свої коефіцієнти в задачу!
                foreach (var appConstraint in appConstraints)
                {
                    appConstraint.ApplyToSolver(solver, variables);
                }

                Solver.ResultStatus resultStatus = solver.Solve();

                // 3. Формування результату
                if (resultStatus == Solver.ResultStatus.OPTIMAL)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Сформований кошик:\n");
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

                            // Якщо це ваговий продукт, показуємо в грамах (множимо на 100)
                            if (product.GetType().Name == "WeightProduct")
                            {
                                displayQuantity = quantity * 100;
                                unit = "г/тижд";
                            }
                            // Якщо поштучний - залишаємо як є
                            else if (product.GetType().Name == "UnitProduct")
                            {
                                unit = "шт/тижд";
                            }

                            sb.AppendLine($"- {product.Name} — {Math.Round(displayQuantity, 0)} {unit}");
                        }
                    }
                    
                    sb.AppendLine("\nЗагальні показники на ТИЖДЕНЬ:");
                    sb.AppendLine($"- Калорій: {Math.Round(totalCalories, 1)} ккал");
                    sb.AppendLine($"- Білків: {Math.Round(totalProteins, 1)} г");
                    sb.AppendLine($"- Жирів: {Math.Round(totalFats, 1)} г");
                    sb.AppendLine($"- Вуглеводів: {Math.Round(totalCarbs, 1)} г");

                    sb.AppendLine($"\nЗагальна ціна: {Math.Round(totalCost, 2)} грн.");

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