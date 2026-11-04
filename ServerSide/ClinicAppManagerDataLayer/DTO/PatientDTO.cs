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
    public class PatientDTO
    {
        public int? PatientId { get; set; }
        public string? AspNetUserId { get; set; }
        public string? BloodType { get; set; }
        public int? PersonId { get; set; }
        public PersonDTO Person { get; set; } = new PersonDTO();

      
    }
    public class DetailPatientDTO
    {
        public PatientDTO Patient { get; set; } = new PatientDTO();
        // Navigation property for appointments
        public ICollection<AppointmentDTO> Appoinments { get; set; } = new List<AppointmentDTO>();
    }
}
