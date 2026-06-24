using System.Windows;
using VehicleRegistryWPF.ViewModels;

namespace VehicleRegistryWPF.Views
{
    public partial class AddInsuranceWindow : Window
    {
        public AddInsuranceWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is AddInsuranceViewModel vm)
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