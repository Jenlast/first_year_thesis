using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace kcalCalculator.Models
{
    public abstract class BaseProduct : INotifyPropertyChanged
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private double _price;
        public double Price { get => _price; set { _price = value; OnPropertyChanged(); } }

        private double _calories;
        public double Calories { get => _calories; set { _calories = value; OnPropertyChanged(); } }

        private double _proteins;
        public double Proteins { get => _proteins; set { _proteins = value; OnPropertyChanged(); } }

        private double _fats;
        public double Fats { get => _fats; set { _fats = value; OnPropertyChanged(); } }

        private double _carbs;
        public double Carbs { get => _carbs; set { _carbs = value; OnPropertyChanged(); } }

        private double _minQuantity = 0;
        public double MinQuantity { get => _minQuantity; set { _minQuantity = value; OnPropertyChanged(); } }

        private double _maxQuantity = 1000;
        public double MaxQuantity { get => _maxQuantity; set { _maxQuantity = value; OnPropertyChanged(); } }

        public abstract string GetProductType();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}