using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VehicleRegistryWPF.Models
{
    public class VehicleRegistry
    {
        private readonly AppDbContext _context;

        public VehicleRegistry(AppDbContext context)
        {
            _context = context;
        }

        public bool RegisterVehicle(Vehicle vehicle)
        {
            if (string.IsNullOrWhiteSpace(vehicle.RegistrationNumber))
                throw new ArgumentException("Numer rejestracyjny nie moze byc pusty");
            if (string.IsNullOrWhiteSpace(vehicle.Brand))
                throw new ArgumentException("Marka nie moze byc pusta");
            if (vehicle.ProductionYear < 1886 || vehicle.ProductionYear > DateTime.Now.Year)
                throw new ArgumentException("Nieprawidlowy rok produkcji");

            var existing = _context.Vehicles
                .FirstOrDefault(v => v.RegistrationNumber == vehicle.RegistrationNumber);

            if (existing != null)
            {
                if (existing.IsRegistered)
                    throw new InvalidOperationException("Pojazd jest juz zarejestrowany");

                existing.Brand = vehicle.Brand;
                existing.Model = vehicle.Model;
                existing.ProductionYear = vehicle.ProductionYear;
                existing.Owner = vehicle.Owner;
                existing.Vin = vehicle.Vin;
                existing.Mileage = vehicle.Mileage;
                existing.InspectionValidUntil = vehicle.InspectionValidUntil;
                existing.InsuranceValidUntil = vehicle.InsuranceValidUntil;
                existing.IsRegistered = true;

                _context.VehicleHistories.Add(new VehicleHistory
                {
                    VehicleId = existing.Id,
                    EventDate = DateTime.Now,
                    EventType = "Registration",
                    AdditionalInfo = $"Ponownie zarejestrowano {existing.Brand} {existing.Model}"
                });
                return _context.SaveChanges() > 0;
            }

            vehicle.IsRegistered = true;
            _context.Vehicles.Add(vehicle);
            _context.VehicleHistories.Add(new VehicleHistory
            {
                Vehicle = vehicle,
                EventDate = DateTime.Now,
                EventType = "Registration",
                AdditionalInfo = $"Zarejestrowano {vehicle.Brand} {vehicle.Model}"
            });
            return _context.SaveChanges() > 0;
        }

        public List<Vehicle> GetAllVehicles()
        {
            return _context.Vehicles.Include(v => v.HistoryEntries).ToList();
        }

        public Vehicle? FindByRegistrationNumber(string regNumber)
        {
            return _context.Vehicles.Include(v => v.HistoryEntries)
                   .FirstOrDefault(v => v.RegistrationNumber == regNumber);
        }

        public void AddInspection(string regNumber, DateTime validUntil)
        {
            var vehicle = _context.Vehicles
                .FirstOrDefault(v => v.RegistrationNumber == regNumber);
            if (vehicle == null) throw new ArgumentException("Pojazd nie istnieje");

            vehicle.InspectionValidUntil = validUntil;
            _context.VehicleHistories.Add(new VehicleHistory
            {
                VehicleId = vehicle.Id,
                EventDate = DateTime.Now,
                EventType = "Inspection",
                ValidUntil = validUntil,
                AdditionalInfo = $"Przeglad wazny do {validUntil:yyyy-MM-dd}"
            });
            _context.SaveChanges();
        }

        public void UpdateInsurance(string regNumber, DateTime validUntil)
        {
            var vehicle = _context.Vehicles
                .FirstOrDefault(v => v.RegistrationNumber == regNumber);
            if (vehicle == null) throw new ArgumentException("Pojazd nie istnieje");

            vehicle.InsuranceValidUntil = validUntil;
            _context.VehicleHistories.Add(new VehicleHistory
            {
                VehicleId = vehicle.Id,
                EventDate = DateTime.Now,
                EventType = "Insurance",
                ValidUntil = validUntil,
                AdditionalInfo = $"Ubezpieczenie wazne do {validUntil:yyyy-MM-dd}"
            });
            _context.SaveChanges();
        }

        public bool UnregisterVehicle(string regNumber)
        {
            var vehicle = FindByRegistrationNumber(regNumber);
            if (vehicle == null) throw new ArgumentException("Pojazd nie istnieje");
            vehicle.IsRegistered = false;
            _context.VehicleHistories.Add(new VehicleHistory
            {
                VehicleId = vehicle.Id,
                EventDate = DateTime.Now,
                EventType = "Unregistration",
                AdditionalInfo = $"Wyrejestrowano pojazd {regNumber}"
            });
            return _context.SaveChanges() > 0;
        }

        public List<VehicleHistory> GetVehicleHistory(string regNumber)
        {
            var vehicle = _context.Vehicles
                .FirstOrDefault(v => v.RegistrationNumber == regNumber);
            if (vehicle == null) return new List<VehicleHistory>();

            return _context.VehicleHistories
                .Where(h => h.VehicleId == vehicle.Id)
                .OrderByDescending(h => h.EventDate)
                .ToList();
        }

        public bool DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle == null) throw new ArgumentException("Pojazd nie istnieje");
            _context.Vehicles.Remove(vehicle);
            return _context.SaveChanges() > 0;
        }

        public void UpdateVehicle(Vehicle updatedVehicle)
        {
            var existing = _context.Vehicles.FirstOrDefault(v => v.Id == updatedVehicle.Id);
            if (existing == null)
                throw new ArgumentException("Pojazd nie istnieje");

            existing.RegistrationNumber = updatedVehicle.RegistrationNumber;
            existing.Brand = updatedVehicle.Brand;
            existing.Model = updatedVehicle.Model;
            existing.ProductionYear = updatedVehicle.ProductionYear;
            existing.Owner = updatedVehicle.Owner;
            existing.Vin = updatedVehicle.Vin;
            existing.Mileage = updatedVehicle.Mileage;
            existing.InspectionValidUntil = updatedVehicle.InspectionValidUntil;
            existing.InsuranceValidUntil = updatedVehicle.InsuranceValidUntil;

            _context.VehicleHistories.Add(new VehicleHistory
            {
                VehicleId = existing.Id,
                EventDate = DateTime.Now,
                EventType = "Edit",
                AdditionalInfo = "Zmodyfikowano dane pojazdu"
            });
            _context.SaveChanges();
        }
    }
}