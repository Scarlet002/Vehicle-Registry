using System.Windows;
using VehicleRegistryWPF.ViewModels;

namespace VehicleRegistryWPF.Views
{
    public partial class AddVehicleWindow : Window
    {
        public AddVehicleWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is AddVehicleViewModel vm)
                {
                    vm.RequestClose += result =>
                    {
                        DialogResult = result;
                        Close();
                    };
                }
            };
        }
    }
}