using System.Windows;
using VehicleRegistryWPF.ViewModels;

namespace VehicleRegistryWPF.Views
{
    public partial class AddInspectionWindow : Window
    {
        public AddInspectionWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is AddInspectionViewModel vm)
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