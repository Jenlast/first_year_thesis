using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using kcalCalculator.ViewModels;
using System.Linq;

namespace kcalCalculator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnOpenMenuClick(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return; // ЗАХИСТ ВІД NULL

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Відкрити базу продуктів",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("JSON файли") { Patterns = new[] { "*.json" } } }
            });

            if (files.Count >= 1)
            {
                string filePath = files[0].Path.LocalPath;
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.LoadDatabase(filePath);
                }
            }
        }

        private async void OnSaveAsMenuClick(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return; // ЗАХИСТ ВІД NULL

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Зберегти базу продуктів",
                DefaultExtension = "json",
                SuggestedFileName = "my_products.json",
                FileTypeChoices = new[] { new FilePickerFileType("JSON файли") { Patterns = new[] { "*.json" } } }
            });

            if (file != null)
            {
                string filePath = file.Path.LocalPath;
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.SaveDatabase(filePath);
                }
            }
        }
    }
}