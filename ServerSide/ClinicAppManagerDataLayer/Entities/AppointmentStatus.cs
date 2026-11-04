using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class AppointmentStatus
    {
        public int AppointmentStatusId { get; set; }

        [MaxLength(50)]
        public string StatusName { get; set; } // e.g., "Scheduled", "Completed", "Cancelled"
    }
}
