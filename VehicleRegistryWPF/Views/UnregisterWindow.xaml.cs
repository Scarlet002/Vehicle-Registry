using System.Windows;
using VehicleRegistryWPF.ViewModels;

namespace VehicleRegistryWPF.Views
{
    public partial class UnregisterWindow : Window
    {
        public UnregisterWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is UnregisterViewModel vm)
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