using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [MaxLength(50)]
        public string Specialization { get; set; }

        [Required]
        [ForeignKey("AspNetUser")]
        public string AspNetUserId { get; set; }

        public AppUser AspNetUser { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person Person { get; set; }

        // Navigation property for appointments
        public ICollection<Appointment> Appoinment { get; set; }
    }
}
