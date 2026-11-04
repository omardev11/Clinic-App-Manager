using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }

        [MaxLength(100)]
        public string VisitDescription { get; set; } // Description of the visit

        [MaxLength(100)]
        public string Diagnosis { get; set; } // Diagnosis made during the visit

        [MaxLength(150)]
        public string AdditionalNotes { get; set; } // Any additional notes from the doctor

        [ForeignKey("Appoinment")]
        public int AppoinmentId { get; set; } // Foreign key to the appointment
        public Appointment Appoinment { get; set; } // Navigation property to the appointment
        public ICollection<PrescribedMedication> PrescribedMedications { get; set; }
    }
}
