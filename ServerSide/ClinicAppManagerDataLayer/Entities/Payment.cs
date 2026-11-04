using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; } 
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }

        [MaxLength(150)]
        public string AdditionalNotes { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } // Navigation property to the appointment

        [ForeignKey("PaymentMethod")]
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } // Navigation property to the payment method
    }
}
