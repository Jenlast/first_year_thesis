using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace kcalCalculator.Models
{
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
        public double MinQuantity { get => _minQuantity; set { if (_minQuantity == value) return; _minQuantity = value; OnPropertyChanged(); } }

        private double _maxQuantity = 1000;
        public double MaxQuantity { get => _maxQuantity; set { if (_maxQuantity == value) return; _maxQuantity = value; OnPropertyChanged(); } }

        public abstract string GetProductType();
        public abstract string MeasurementType { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}