using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using VehicleRegistryWPF.Models;

namespace VehicleRegistryWPF.ViewModels
{
    public class AddVehicleViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly VehicleRegistry _registry;
        private Vehicle _newVehicle;

        public AddVehicleViewModel()
        {
            _registry = App.Registry;
            _newVehicle = new Vehicle();
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public Vehicle NewVehicle
        {
            get => _newVehicle;
            set { _newVehicle = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? RequestClose;

        private bool CanSave() => string.IsNullOrEmpty(Error);

        private void Save()
        {
            try
            {
                _registry.RegisterVehicle(NewVehicle);
                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string Error => null!;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(NewVehicle.RegistrationNumber):
                        if (string.IsNullOrWhiteSpace(NewVehicle.RegistrationNumber))
                            error = "Numer rejestracyjny jest wymagany.";
                        break;
                    case nameof(NewVehicle.Brand):
                        if (string.IsNullOrWhiteSpace(NewVehicle.Brand))
                            error = "Marka jest wymagana.";
                        break;
                    case nameof(NewVehicle.Model):
                        if (string.IsNullOrWhiteSpace(NewVehicle.Model))
                            error = "Model jest wymagany.";
                        break;
                    case nameof(NewVehicle.ProductionYear):
                        if (NewVehicle.ProductionYear < 1886 || NewVehicle.ProductionYear > DateTime.Now.Year)
                            error = $"Rok produkcji musi być między 1886 a {DateTime.Now.Year}.";
                        break;
                    case nameof(NewVehicle.Owner):
                        if (string.IsNullOrWhiteSpace(NewVehicle.Owner))
                            error = "Właściciel jest wymagany.";
                        break;
                    case nameof(NewVehicle.Vin):
                        if (!string.IsNullOrWhiteSpace(NewVehicle.Vin) && NewVehicle.Vin.Length != 17)
                            error = "VIN musi mieć dokładnie 17 znaków.";
                        break;
                    case nameof(NewVehicle.Mileage):
                        if (NewVehicle.Mileage.HasValue && NewVehicle.Mileage < 0)
                            error = "Przebieg nie może być ujemny.";
                        break;
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}