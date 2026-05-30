using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using kcalCalculator.Models;

namespace kcalCalculator.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly DataManager _dataManager;
        private readonly OptimizationService _optimizationService;

        public ObservableCollection<BaseProduct> Products { get; set; }
        public ObservableCollection<BaseProduct> FilteredProducts { get; set; }
        public ConstraintsContext UserConstraints { get; set; }

        private string _calculationResult = "Тут буде результат...";
        public string CalculationResult
        {
            get => _calculationResult;
            set { _calculationResult = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set 
            { 
                _searchText = value; 
                OnPropertyChanged(); 
                ApplyFilter();
            }
        }

        private BaseProduct? _selectedProduct;
        public BaseProduct? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public ICommand CalculateBasketCommand { get; }
        public ICommand AddWeightProductCommand { get; }
        public ICommand AddUnitProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ExitCommand { get; } 

        public MainWindowViewModel()
        {
            _dataManager = new DataManager();
            _optimizationService = new OptimizationService();
            
            // --- Безпечне завантаження при старті ---
            try
            {
                var loaded = _dataManager.LoadProducts("products_db.json");
                Products = new ObservableCollection<BaseProduct>(loaded);
            }
            catch
            {
                // Якщо файл зламаний, старий або його немає - створюємо порожній список
                Products = new ObservableCollection<BaseProduct>();
            }

            // Якщо база порожня, додаємо стандартні
            if (Products.Count == 0)
            {
                Products.Add(new WeightProduct { Name = "Гречка", Price = 7.5, Calories = 330, Proteins = 12.6, Fats = 3.3, Carbs = 62, MinQuantity = 10, MaxQuantity = 50 });
                Products.Add(new WeightProduct { Name = "Куряче філе", Price = 21.5, Calories = 110, Proteins = 23, Fats = 1.5, Carbs = 0, MinQuantity = 10, MaxQuantity = 40 });
                Products.Add(new UnitProduct { Name = "Яйця (1шт)", Price = 5.4, Calories = 75, Proteins = 6, Fats = 5, Carbs = 0.5, MinQuantity = 15, MaxQuantity = 40 });
            }

            FilteredProducts = new ObservableCollection<BaseProduct>(Products);

            UserConstraints = new ConstraintsContext
            {
                MaxBudget = 10000m,
                MinProteins = 900m, MaxProteins = 1960m,
                MinFats = 300m, MaxFats = 980m,
                MinCarbs = 1500m, MaxCarbs = 3000m
            };

            CalculateBasketCommand = new RelayCommand(_ => Calculate());
            AddWeightProductCommand = new RelayCommand(_ => AddWeightProduct());
            AddUnitProductCommand = new RelayCommand(_ => AddUnitProduct());
            DeleteProductCommand = new RelayCommand(_ => DeleteProduct());
            ExitCommand = new RelayCommand(_ => Environment.Exit(0));

            UpdateStatus();
        }

        private void ApplyFilter()
        {
            FilteredProducts.Clear();
            foreach (var product in Products)
            {
                if (string.IsNullOrWhiteSpace(SearchText) || 
                    product.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                {
                    FilteredProducts.Add(product);
                }
            }
        }

        private void Calculate()
        {
            StatusMessage = "Проводиться розрахунок...";
            CalculationResult = _optimizationService.CalculateOptimalBasket(Products, UserConstraints);
            UpdateStatus();
        }

        private void AddWeightProduct()
        {
            var newProd = new WeightProduct { Name = "Новий продукт (ваговий)", MaxQuantity = 10 };
            Products.Add(newProd);
            ApplyFilter();
            UpdateStatus();
        }

        private void AddUnitProduct()
        {
            var newProd = new UnitProduct { Name = "Новий продукт (штучний)", MaxQuantity = 10 };
            Products.Add(newProd);
            ApplyFilter();
            UpdateStatus();
        }

        private void DeleteProduct()
        {
            if (SelectedProduct != null)
            {
                string deletedName = SelectedProduct.Name;
                Products.Remove(SelectedProduct);
                ApplyFilter();
                CalculationResult = $"Продукт '{deletedName}' видалено.";
                UpdateStatus();
            }
        }

        public void LoadDatabase(string filePath)
        {
            try
            {
                var loaded = _dataManager.LoadProducts(filePath);
                Products.Clear();
                foreach (var item in loaded) Products.Add(item);
                
                ApplyFilter();
                CalculationResult = "Базу успішно завантажено з файлу!";
                UpdateStatus($"Завантажено файл. Продуктів: {Products.Count}");
            }
            catch (Exception ex)
            {
                CalculationResult = $"Помилка завантаження: {ex.Message}";
            }
        }

        public void SaveDatabase(string filePath)
        {
            try
            {
                _dataManager.SaveProducts(filePath, Products);
                CalculationResult = "Базу успішно збережено!";
                UpdateStatus("Базу збережено!");
            }
            catch (Exception ex)
            {
                CalculationResult = $"Помилка збереження: {ex.Message}";
            }
        }

        private void UpdateStatus(string? customMessage = null)
        {
            if (customMessage != null)
                StatusMessage = customMessage;
            else
                StatusMessage = $"Готовий до роботи. Всього продуктів у базі: {Products.Count}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        public RelayCommand(Action<object?> execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute(parameter);
    }
}