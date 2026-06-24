using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using VehicleRegistryWPF.Models;

namespace VehicleRegistryWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly VehicleRegistry _registry;
        private ObservableCollection<Vehicle> _vehicles;
        private string _searchText = string.Empty;
        private Vehicle? _selectedVehicle;

        public ObservableCollection<Vehicle> Vehicles
        {
            get => _vehicles;
            set { _vehicles = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); SearchCommand.Execute(null); }
        }

        public Vehicle? SelectedVehicle
        {
            get => _selectedVehicle;
            set { _selectedVehicle = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand UnregisterCommand { get; }
        public ICommand InspectionCommand { get; }
        public ICommand InsuranceCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ShowAllCommand { get; }
        public ICommand ToggleRegistrationCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            _registry = App.Registry;
            LoadVehicles();

            AddCommand = new RelayCommand(_ => AddVehicle());
            UnregisterCommand = new RelayCommand(_ => UnregisterVehicle(), _ => SelectedVehicle != null);
            InspectionCommand = new RelayCommand(_ => AddInspection(), _ => SelectedVehicle != null);
            InsuranceCommand = new RelayCommand(_ => UpdateInsurance(), _ => SelectedVehicle != null);
            HistoryCommand = new RelayCommand(_ => ShowHistory(), _ => SelectedVehicle != null);
            RefreshCommand = new RelayCommand(_ => LoadVehicles());
            SearchCommand = new RelayCommand(_ => PerformSearch());
            ShowAllCommand = new RelayCommand(_ => LoadVehicles());
            ToggleRegistrationCommand = new RelayCommand(param => ToggleRegistration(param as Vehicle));
            DeleteCommand = new RelayCommand(_ => DeleteVehicle(), _ => SelectedVehicle != null);
        }

        private void LoadVehicles()
        {
            Vehicles = new ObservableCollection<Vehicle>(_registry.GetAllVehicles());
        }

        private void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadVehicles();
                return;
            }

            var all = _registry.GetAllVehicles();
            var lowerSearch = SearchText.ToLowerInvariant();
            var filtered = all.Where(v =>
                v.RegistrationNumber.ToLowerInvariant().Contains(lowerSearch) ||
                (v.Vin != null && v.Vin.ToLowerInvariant().Contains(lowerSearch)) ||
                v.Brand.ToLowerInvariant().Contains(lowerSearch) ||
                v.Model.ToLowerInvariant().Contains(lowerSearch) ||
                v.ProductionYear.ToString().Contains(lowerSearch) ||
                v.Owner.ToLowerInvariant().Contains(lowerSearch) ||
                (v.InspectionValidUntil.HasValue && v.InspectionValidUntil.Value.ToString("yyyy-MM-dd").Contains(lowerSearch)) ||
                (v.InsuranceValidUntil.HasValue && v.InsuranceValidUntil.Value.ToString("yyyy-MM-dd").Contains(lowerSearch)) ||
                (v.Mileage.HasValue && v.Mileage.Value.ToString().Contains(lowerSearch))
            ).ToList();

            Vehicles = new ObservableCollection<Vehicle>(filtered);
        }

        private void AddVehicle()
        {
            var vm = new AddVehicleViewModel();
            var win = new Views.AddVehicleWindow { DataContext = vm };
            if (win.ShowDialog() == true)
                LoadVehicles();
        }

        private void UnregisterVehicle()
        {
            if (SelectedVehicle == null) return;
            var vm = new UnregisterViewModel(SelectedVehicle.RegistrationNumber);
            var win = new Views.UnregisterWindow { DataContext = vm };
            if (win.ShowDialog() == true)
                LoadVehicles();
        }

        private void AddInspection()
        {
            if (SelectedVehicle == null) return;
            var vm = new AddInspectionViewModel(SelectedVehicle.RegistrationNumber);
            var win = new Views.AddInspectionWindow { DataContext = vm };
            if (win.ShowDialog() == true)
                LoadVehicles();
        }

        private void UpdateInsurance()
        {
            if (SelectedVehicle == null) return;
            var vm = new AddInsuranceViewModel(SelectedVehicle.RegistrationNumber);
            var win = new Views.AddInsuranceWindow { DataContext = vm };
            if (win.ShowDialog() == true)
                LoadVehicles();
        }

        private void ShowHistory()
        {
            if (SelectedVehicle == null) return;
            var win = new Views.HistoryWindow(SelectedVehicle.RegistrationNumber);
            win.ShowDialog();
        }

        private void ToggleRegistration(Vehicle? vehicle)
        {
            if (vehicle == null) return;
            try
            {
                if (vehicle.IsRegistered)
                    _registry.UnregisterVehicle(vehicle.RegistrationNumber);
                else
                    _registry.RegisterVehicle(vehicle);
                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadVehicles();
            }
        }

        private void DeleteVehicle()
        {
            if (SelectedVehicle == null) return;
            var result = MessageBox.Show($"Czy na pewno usunąć pojazd {SelectedVehicle.RegistrationNumber}?",
                                         "Potwierdzenie usunięcia",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _registry.DeleteVehicle(SelectedVehicle.Id);
                    LoadVehicles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}