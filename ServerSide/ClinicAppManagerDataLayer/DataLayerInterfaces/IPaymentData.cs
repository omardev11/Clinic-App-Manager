using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IPaymentData
    {
        public Task<int> AddPaymentAsync(Payment payment);
        public Task<bool> UpdatePaymentAsync(PaymentDTO payment); 
        public Task<Payment> GetPaymentByIdAsync(int PaymentId);
        public Task<Payment> GetPaymentByAppointmentIdAsync(int AppointemtId);

        public Task<List<Payment>> GetAllPaymentsAsync();

    }
}
