using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using kcalCalculator.Models;

namespace kcalCalculator.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly DataManager _dataManager;
        private readonly OptimizationService _optimizationService;

        // Список продуктів для DataGrid
        public ObservableCollection<BaseProduct> Products { get; set; }
        
        // Обмеження користувача
        public ConstraintsContext UserConstraints { get; set; }

        // Результат розрахунку
        private string _calculationResult = "Тут буде результат...";
        public string CalculationResult
        {
            get => _calculationResult;
            set
            {
                _calculationResult = value;
                OnPropertyChanged();
            }
        }

        // Команди для кнопок
        public ICommand CalculateBasketCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddProductCommand { get; }
        // Команда для видалення
        public ICommand DeleteProductCommand { get; }

        // Властивість для збереження виділеного продукту в таблиці
        private BaseProduct _selectedProduct;
        public BaseProduct SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            _dataManager = new DataManager();
            _optimizationService = new OptimizationService();
            
            var loaded = _dataManager.LoadProducts();
            Products = new ObservableCollection<BaseProduct>(loaded);

            if (Products.Count == 0)
            {
                Products.Add(new WeightProduct { Name = "Гречка", Price = 40, Calories = 330, Proteins = 12.6, Fats = 3.3, Carbs = 57.1, MinQuantity = 0, MaxQuantity = 10 });
                Products.Add(new WeightProduct { Name = "Куряче філе", Price = 150, Calories = 110, Proteins = 23, Fats = 1.2, Carbs = 0, MinQuantity = 0, MaxQuantity = 5 });
                Products.Add(new WeightProduct { Name = "Яйця (1шт)", Price = 6, Calories = 78, Proteins = 6, Fats = 5, Carbs = 0.6, MinQuantity = 0, MaxQuantity = 30 });
            }

            UserConstraints = new ConstraintsContext
            {
                MaxBudget = 500,
                MinProteins = 50, MaxProteins = 150,
                MinFats = 40, MaxFats = 80,
                MinCarbs = 150, MaxCarbs = 300
            };

            CalculateBasketCommand = new RelayCommand(_ => Calculate());
            SaveCommand = new RelayCommand(_ => SaveData());
            AddProductCommand = new RelayCommand(_ => AddProduct());
            DeleteProductCommand = new RelayCommand(_ => DeleteProduct());
        }

        private void Calculate()
        {
            CalculationResult = _optimizationService.CalculateOptimalBasket(Products, UserConstraints);
        }

        private void SaveData()
        {
            try
            {
                var listToSave = new System.Collections.Generic.List<WeightProduct>();
                foreach (var p in Products)
                {
                    if (p is WeightProduct wp) listToSave.Add(wp);
                    else listToSave.Add(new WeightProduct { Name = p.Name, Price = p.Price, Calories = p.Calories, Proteins = p.Proteins, Fats = p.Fats, Carbs = p.Carbs, MinQuantity = p.MinQuantity, MaxQuantity = p.MaxQuantity });
                }
                
                _dataManager.SaveProducts(listToSave);
                CalculationResult = "База продуктів успішно збережена у файл!";
            }
            catch (Exception ex)
            {
                CalculationResult = $"Помилка збереження: {ex.Message}";
            }
        }

        private void AddProduct()
        {
            Products.Add(new WeightProduct { Name = "Новий продукт", MaxQuantity = 10 });
        }
        private void DeleteProduct()
        {
            if (SelectedProduct != null)
            {
                Products.Remove(SelectedProduct);
                CalculationResult = $"Продукт '{SelectedProduct.Name}' видалено.";
            }
            else
            {
                CalculationResult = "Будь ласка, спочатку виділіть продукт у таблиці для видалення.";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Клас для команд
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public RelayCommand(Action<object> execute) => _execute = execute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);
    }
}