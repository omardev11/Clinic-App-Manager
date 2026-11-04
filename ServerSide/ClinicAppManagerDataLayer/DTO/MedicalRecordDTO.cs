using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DTO
{
    public class MedicalRecordDTO
    {
        public int? RecordId { get; set; }
        public string? VisitDescription { get; set; } // Description of the visit
        public string? Diagnosis { get; set; } // Diagnosis made during the visit
        public string? AdditionalNotes { get; set; } // Any additional notes from the doctor
        public int? AppoinmentId { get; set; } // Foreign key to the appointment
    }
    
}
