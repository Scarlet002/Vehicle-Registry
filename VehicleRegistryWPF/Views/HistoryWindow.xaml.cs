using System.Windows;
using VehicleRegistryWPF.Models;

namespace VehicleRegistryWPF.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(string regNumber)
        {
            InitializeComponent();
            TitleLabel.Text = $"Historia pojazdu {regNumber}";
            HistoryGrid.ItemsSource = App.Registry.GetVehicleHistory(regNumber);
        }
    }
}