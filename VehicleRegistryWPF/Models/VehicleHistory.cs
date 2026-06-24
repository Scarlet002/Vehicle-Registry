using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRegistryWPF.Models
{
    public class VehicleHistory
    {
        [Key]
        public int Id { get; set; }

        public int VehicleId { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required, MaxLength(50)]
        public string EventType { get; set; } = string.Empty;

        public DateTime? ValidUntil { get; set; }

        [MaxLength(500)]
        public string? AdditionalInfo { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; } = null!;
    }
}