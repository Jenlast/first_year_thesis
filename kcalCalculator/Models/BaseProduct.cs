using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace kcalCalculator.Models
{
    // <summary>
    // Абстрактний базовий клас, що представляє загальні характеристики будь-якого харчового продукту.
    // Демонструє принцип інкапсуляції (приватні поля з перевіркою) та є основою для поліморфізму.
    // </summary>
    [JsonDerivedType(typeof(WeightProduct), typeDiscriminator: "weight")]
    [JsonDerivedType(typeof(UnitProduct), typeDiscriminator: "unit")]
    public abstract class BaseProduct : INotifyPropertyChanged
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        private bool _isMandatory;
        public bool IsMandatory { get => _isMandatory; set { if (_isMandatory == value) return; _isMandatory = value; OnPropertyChanged(); } }

        private string _name = string.Empty;
        public string Name { get => _name; set { if (_name == value) return; _name = value; OnPropertyChanged(); } }

        private double _price;
        public double Price { get => _price; set { if (_price == value) return; _price = value; OnPropertyChanged(); } }

        private double _calories;
        public double Calories { get => _calories; set { if (_calories == value) return; _calories = value; OnPropertyChanged(); } }

        private double _proteins;
        public double Proteins { get => _proteins; set { if (_proteins == value) return; _proteins = value; OnPropertyChanged(); } }

        private double _fats;
        public double Fats { get => _fats; set { if (_fats == value) return; _fats = value; OnPropertyChanged(); } }

        private double _carbs;
        public double Carbs { get => _carbs; set { if (_carbs == value) return; _carbs = value; OnPropertyChanged(); } }

        private double _minQuantity = 0;
        public double MinQuantity 
        { 
            get => _minQuantity; 
            set 
            { 
                if (_minQuantity == value) return; 
                _minQuantity = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(DisplayMinQuantity));
            } 
        }

        private double _maxQuantity = 1000;
        public double MaxQuantity 
        { 
            get => _maxQuantity; 
            set 
            { 
                if (_maxQuantity == value) return; 
                _maxQuantity = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(DisplayMaxQuantity));
            } 
        }

        [JsonIgnore]
        public abstract double DisplayMinQuantity { get; set; }

        [JsonIgnore]
        public abstract double DisplayMaxQuantity { get; set; }

        [JsonIgnore]
        public abstract string UnitName { get; }

        // <summary>
        /// Абстрактний метод для отримання типу продукту (ваговий чи поштучний).
        /// </summary>
        /// <returns>Рядок з назвою типу продукту.</returns>
        public abstract string GetProductType();

        // <summary>
        // Абстрактна властивість, що повертає одиницю виміру продукту.
        // Реалізується в дочірніх класах (поліморфізм).
        // </summary>
        public abstract string MeasurementType { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        // <summary>
        /// Метод для сповіщення графічного інтерфейсу про зміну значення властивості.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}