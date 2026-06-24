using System;
using System.Windows;
using System.Windows.Controls;
using VehicleRegistryWPF.Models;
using VehicleRegistryWPF.ViewModels;

namespace VehicleRegistryWPF
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void VehiclesGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            var editedVehicle = e.Row.Item as Vehicle;
            if (editedVehicle == null)
                return;

            var textBox = e.EditingElement as TextBox;
            if (textBox == null)
                return;

            var column = e.Column as DataGridTextColumn;
            if (column?.Binding is not System.Windows.Data.Binding binding)
                return;

            string propertyName = binding.Path.Path;
            if (string.IsNullOrEmpty(propertyName))
                return;

            var propInfo = editedVehicle.GetType().GetProperty(propertyName);
            if (propInfo == null || !propInfo.CanWrite)
                return;

            try
            {
                object value = Convert.ChangeType(textBox.Text, propInfo.PropertyType);
                propInfo.SetValue(editedVehicle, value);
                App.Registry.UpdateVehicle(editedVehicle);
            }
            catch (FormatException)
            {
                MessageBox.Show($"Nieprawid³owy format dla pola {propertyName}.", "B³¹d walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"B³¹d zapisu: {ex.Message}", "B³¹d", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Cancel = true;
            }
        }
    }
}