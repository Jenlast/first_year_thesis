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

        // Повний список продуктів (база)
        public ObservableCollection<BaseProduct> Products { get; set; }
        
        public ObservableCollection<BaseProduct> FilteredProducts { get; set; }

        public ConstraintsContext UserConstraints { get; set; }

        private string _calculationResult = "Тут буде результат...";
        public string CalculationResult
        {
            get => _calculationResult;
            set { _calculationResult = value; OnPropertyChanged(); }
        }

        private string _statusMessage;
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
                ApplyFilter(); // Одразу фільтруємо таблицю при введенні тексту
            }
        }

        private BaseProduct _selectedProduct;
        public BaseProduct SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public ICommand CalculateBasketCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ExitCommand { get; } 

        public MainWindowViewModel()
        {
            _dataManager = new DataManager();
            _optimizationService = new OptimizationService();
            
            var loaded = _dataManager.LoadProducts();
            Products = new ObservableCollection<BaseProduct>(loaded);

            if (Products.Count == 0)
            {
                Products.Add(new WeightProduct { Name = "Гречка", Price = 7.5, Calories = 330, Proteins = 12.6, Fats = 3.3, Carbs = 62, MinQuantity = 10, MaxQuantity = 50 });
                Products.Add(new WeightProduct { Name = "Куряче філе", Price = 21.5, Calories = 110, Proteins = 23, Fats = 1.5, Carbs = 0, MinQuantity = 10, MaxQuantity = 40 });
                Products.Add(new WeightProduct { Name = "Яйця (1шт)", Price = 5.4, Calories = 75, Proteins = 6, Fats = 5, Carbs = 0.5, MinQuantity = 15, MaxQuantity = 40 });
            }

            // Ініціалізуємо відфільтрований список
            FilteredProducts = new ObservableCollection<BaseProduct>(Products);

            UserConstraints = new ConstraintsContext
            {
                MaxBudget = 10000,
                MinProteins = 900, MaxProteins = 1960,
                MinFats = 300, MaxFats = 980,
                MinCarbs = 1500, MaxCarbs = 3000
            };

            CalculateBasketCommand = new RelayCommand(_ => Calculate());
            SaveCommand = new RelayCommand(_ => SaveData());
            AddProductCommand = new RelayCommand(_ => AddProduct());
            DeleteProductCommand = new RelayCommand(_ => DeleteProduct());
            
            ExitCommand = new RelayCommand(_ => Environment.Exit(0));

            UpdateStatus();
        }

        private void ApplyFilter()
        {
            FilteredProducts.Clear();
            foreach (var product in Products)
            {
                // Якщо рядок пошуку порожній АБО назва продукту містить текст пошуку (незалежно від регістру)
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

        private void SaveData()
        {
            var listToSave = new System.Collections.Generic.List<WeightProduct>();
            foreach (var p in Products)
            {
                if (p is WeightProduct wp) listToSave.Add(wp);
                else listToSave.Add(new WeightProduct { Name = p.Name, Price = p.Price, Calories = p.Calories, Proteins = p.Proteins, Fats = p.Fats, Carbs = p.Carbs, MinQuantity = p.MinQuantity, MaxQuantity = p.MaxQuantity });
            }
            
            _dataManager.SaveProducts(listToSave);
            CalculationResult = "База продуктів успішно збережена у файл!";
            UpdateStatus("Базу збережено!");
        }

        private void AddProduct()
        {
            var newProd = new WeightProduct { Name = "Новий продукт", MaxQuantity = 10 };
            Products.Add(newProd);
            ApplyFilter(); // Оновлюємо таблицю
            UpdateStatus();
        }

        private void DeleteProduct()
        {
            if (SelectedProduct != null)
            {
                string deletedName = SelectedProduct.Name;
                Products.Remove(SelectedProduct);
                ApplyFilter(); // Оновлюємо таблицю
                CalculationResult = $"Продукт '{deletedName}' видалено.";
                UpdateStatus();
            }
        }

        private void UpdateStatus(string customMessage = null)
        {
            if (customMessage != null)
                StatusMessage = customMessage;
            else
                StatusMessage = $"Готовий до роботи. Всього продуктів у базі: {Products.Count}";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public RelayCommand(Action<object> execute) => _execute = execute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);
    }
}