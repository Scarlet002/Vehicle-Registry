using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VehicleRegistryWPF.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [MaxLength(17)]
        public string? Vin { get; set; }

        [Required, MaxLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(1886, 2100)]
        public int ProductionYear { get; set; }

        public int? Mileage { get; set; }

        [MaxLength(100)]
        public string Owner { get; set; } = string.Empty;

        public bool IsRegistered { get; set; } = true;

        public DateTime? InspectionValidUntil { get; set; }
        public DateTime? InsuranceValidUntil { get; set; }

        public ICollection<VehicleHistory> HistoryEntries { get; set; } = new List<VehicleHistory>();
    }
}