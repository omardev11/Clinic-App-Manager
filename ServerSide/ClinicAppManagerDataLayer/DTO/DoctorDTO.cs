using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DTO
{
    public class DoctorDTO
    {
        public int? DoctorId { get; set; }
        public string? AspNetUserId { get; set; }
        public string? Specialization { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public int? PersonId { get; set; }
        public PersonDTO Person { get; set; } = new PersonDTO();
    }
    public class DetailDoctorDTO
    {
      public DoctorDTO Doctor { get; set; } = new DoctorDTO();

        // Navigation property for appointments
        public ICollection<AppointmentDTO> Appoinments { get; set; } = new List<AppointmentDTO>();

    }
}
