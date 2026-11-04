using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DTO
{
    public class AppointmentStatusDTO
    {
        public int? AppointmentStatusId { get; set; }
        public string? StatusName { get; set; }
    }
}
