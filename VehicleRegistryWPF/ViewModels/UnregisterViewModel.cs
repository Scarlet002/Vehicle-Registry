using System;
using System.Windows;
using System.Windows.Input;
using VehicleRegistryWPF.Models;

namespace VehicleRegistryWPF.ViewModels
{
    public class UnregisterViewModel
    {
        private readonly VehicleRegistry _registry;
        private readonly string _regNumber;

        public UnregisterViewModel(string regNumber)
        {
            _registry = App.Registry;
            _regNumber = regNumber;
            ConfirmCommand = new RelayCommand(_ => Confirm());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public string RegistrationNumber => _regNumber;

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? RequestClose;

        private void Confirm()
        {
            try
            {
                _registry.UnregisterVehicle(_regNumber);
                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}