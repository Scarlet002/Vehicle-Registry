using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using VehicleRegistryWPF.Models;

namespace VehicleRegistryWPF
{
    public partial class App : Application
    {
        public static AppDbContext DbContext { get; private set; }
        public static VehicleRegistry Registry { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            DbContext = new AppDbContext();

            try
            {
                DbContext.Database.Migrate();
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            when (ex.Message.Contains("already exists"))
            {
                DbContext.Dispose();
                DbContext = null!;

                using (var tempContext = new AppDbContext())
                {
                    tempContext.Database.EnsureDeleted();
                }

                DbContext = new AppDbContext();
                DbContext.Database.Migrate();
            }

            Registry = new VehicleRegistry(DbContext);

            if (!DbContext.Vehicles.Any())
            {
                SeedDatabase();
            }

            base.OnStartup(e);
        }

        private void SeedDatabase()
        {
            var vehicles = new[]
            {
                new Vehicle
                {
                    RegistrationNumber = "WAR1234",
                    Vin = "1HGCM82633A123456",
                    Brand = "Toyota",
                    Model = "Corolla",
                    ProductionYear = 2020,
                    Mileage = 45000,
                    Owner = "Jan Kowalski",
                    IsRegistered = true,
                    InspectionValidUntil = DateTime.Now.AddMonths(6),
                    InsuranceValidUntil = DateTime.Now.AddMonths(3)
                },
                new Vehicle
                {
                    RegistrationNumber = "KR5678",
                    Vin = "3FA6P0H77JR123456",
                    Brand = "Ford",
                    Model = "Focus",
                    ProductionYear = 2019,
                    Mileage = 78000,
                    Owner = "Anna Nowak",
                    IsRegistered = true,
                    InspectionValidUntil = DateTime.Now.AddMonths(2),
                    InsuranceValidUntil = DateTime.Now.AddMonths(8)
                },
                new Vehicle
                {
                    RegistrationNumber = "PO9012",
                    Vin = "WBA8A9C55GK123456",
                    Brand = "BMW",
                    Model = "3 Series",
                    ProductionYear = 2021,
                    Mileage = 32000,
                    Owner = "Piotr Wiœniewski",
                    IsRegistered = true,
                    InspectionValidUntil = DateTime.Now.AddMonths(10),
                    InsuranceValidUntil = DateTime.Now.AddMonths(5)
                },
                new Vehicle
                {
                    RegistrationNumber = "GD3456",
                    Vin = "WVWZZZ1KZMW123456",
                    Brand = "Volkswagen",
                    Model = "Golf",
                    ProductionYear = 2018,
                    Mileage = 95000,
                    Owner = "Maria Wojciechowska",
                    IsRegistered = false,
                    InspectionValidUntil = DateTime.Now.AddMonths(-2),
                    InsuranceValidUntil = DateTime.Now.AddMonths(-1)
                }
            };

            foreach (var vehicle in vehicles)
            {
                Registry.RegisterVehicle(vehicle);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DbContext?.Dispose();
            base.OnExit(e);
        }
    }
}