using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DTO
{
    public class AppointmentDTO
    {
        public int? AppointmentId { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateOnly? AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }
        public int? AppointmentStatusId { get; set; }
    }
    public class DetailAppointmentDTO
    {
        public AppointmentDTO Appointment { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public string PatientFullName { get; set; }
        public string DoctorFullName { get; set; }


    }
}
