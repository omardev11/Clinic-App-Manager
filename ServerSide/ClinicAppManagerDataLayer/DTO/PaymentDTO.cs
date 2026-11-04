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
    public class PaymentDTO
    {
        public int? PaymentId { get; set; }
        public decimal? AmountPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? AdditionalNotes { get; set; }
        public int? AppointmentId { get; set; }
        public int? PaymentMethodId { get; set; }
    }
    public class DetailPaymentDTO
    {
        public PaymentDTO PaymentDTO { get; set; }
        public string PaymentMethodName { get; set; }
    }

}
