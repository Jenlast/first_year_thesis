using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace kcalCalculator.Models
{
    public class ConstraintsContext : INotifyPropertyChanged
    {
        private double _maxBudget;
        public double MaxBudget { get => _maxBudget; set { _maxBudget = value; OnPropertyChanged(); } }

        private double _minProteins;
        public double MinProteins { get => _minProteins; set { _minProteins = value; OnPropertyChanged(); } }

        private double _maxProteins;
        public double MaxProteins { get => _maxProteins; set { _maxProteins = value; OnPropertyChanged(); } }

        private double _minFats;
        public double MinFats { get => _minFats; set { _minFats = value; OnPropertyChanged(); } }

        private double _maxFats;
        public double MaxFats { get => _maxFats; set { _maxFats = value; OnPropertyChanged(); } }

        private double _minCarbs;
        public double MinCarbs { get => _minCarbs; set { _minCarbs = value; OnPropertyChanged(); } }

        private double _maxCarbs;
        public double MaxCarbs { get => _maxCarbs; set { _maxCarbs = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}