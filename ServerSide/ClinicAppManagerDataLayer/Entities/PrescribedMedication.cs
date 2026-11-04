using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class PrescribedMedication
    {
        [Key]
        public int PrescriptionId { get; set; } 
        public string MedicatoinName { get; set; } 
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string SpecialInstructions { get; set; }

        [ForeignKey("MedicalRecord")]
        public int RecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; } 
    }
}
