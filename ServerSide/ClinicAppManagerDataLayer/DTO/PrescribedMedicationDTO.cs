using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DTO
{
    public class PrescribedMedicationDTO
    {
        public int? PrescriptionId { get; set; }
        public string? MedicatoinName { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? SpecialInstructions { get; set; }
        public int? RecordId { get; set; }
    }
}
