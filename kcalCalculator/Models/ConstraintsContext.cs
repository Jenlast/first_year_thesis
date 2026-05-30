using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace kcalCalculator.Models
{
    public class ConstraintsContext : INotifyPropertyChanged
    {
        private decimal _maxBudget;
        public decimal MaxBudget { get => _maxBudget; set { if (_maxBudget == value) return; _maxBudget = value; OnPropertyChanged(); } }

        private decimal _minProteins;
        public decimal MinProteins { get => _minProteins; set { if (_minProteins == value) return; _minProteins = value; OnPropertyChanged(); } }

        private decimal _maxProteins;
        public decimal MaxProteins { get => _maxProteins; set { if (_maxProteins == value) return; _maxProteins = value; OnPropertyChanged(); } }

        private decimal _minFats;
        public decimal MinFats { get => _minFats; set { if (_minFats == value) return; _minFats = value; OnPropertyChanged(); } }

        private decimal _maxFats;
        public decimal MaxFats { get => _maxFats; set { if (_maxFats == value) return; _maxFats = value; OnPropertyChanged(); } }

        private decimal _minCarbs;
        public decimal MinCarbs { get => _minCarbs; set { if (_minCarbs == value) return; _minCarbs = value; OnPropertyChanged(); } }

        private decimal _maxCarbs;
        public decimal MaxCarbs { get => _maxCarbs; set { if (_maxCarbs == value) return; _maxCarbs = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}