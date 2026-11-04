using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [MaxLength(15)]
        public string BloodType { get; set; }

        [Required]
        [ForeignKey("AspNetUser")]
        public string AspNetUserId { get; set; }

        public AppUser AspNetUser { get; set; }


        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person Person { get; set; }

        // Navigation property for appointments
        public ICollection<Appointment> Appoinment { get; set; }
    }
}
