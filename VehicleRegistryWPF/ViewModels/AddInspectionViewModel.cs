using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using VehicleRegistryWPF.Models;

namespace VehicleRegistryWPF.ViewModels
{
    public class AddInspectionViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly VehicleRegistry _registry;
        private readonly string _regNumber;
        private DateTime? _validUntil;

        public AddInspectionViewModel(string regNumber)
        {
            _registry = App.Registry;
            _regNumber = regNumber;
            _validUntil = DateTime.Now.AddYears(1);
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public string RegistrationNumber => _regNumber;

        public DateTime? ValidUntil
        {
            get => _validUntil;
            set { _validUntil = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? RequestClose;

        private bool CanSave() => string.IsNullOrEmpty(Error);

        private void Save()
        {
            try
            {
                if (!ValidUntil.HasValue)
                    throw new Exception("Wybierz datę ważności przeglądu.");
                _registry.AddInspection(_regNumber, ValidUntil.Value);
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
                if (columnName == nameof(ValidUntil))
                {
                    if (!ValidUntil.HasValue)
                        error = "Data jest wymagana.";
                    else if (ValidUntil.Value < DateTime.Now.Date)
                        error = "Data nie może być w przeszłości.";
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}