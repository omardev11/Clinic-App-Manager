using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Payments
{
    public class PaymentData : IPaymentData
    {
        private readonly AppDbContext _context;

        public PaymentData(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment.PaymentId;
        }

        public async Task<bool> UpdatePaymentAsync(PaymentDTO payment)
        {
            var existingPayment = await _context.Payments.FindAsync(payment.PaymentId);
            if (existingPayment == null)
                return false;

            existingPayment.AmountPaid = payment.AmountPaid.HasValue ? payment.AmountPaid.Value : existingPayment.AmountPaid;
            existingPayment.PaymentDate = payment.PaymentDate.HasValue ? payment.PaymentDate.Value : existingPayment.PaymentDate;
            existingPayment.AdditionalNotes = payment.AdditionalNotes ?? existingPayment.AdditionalNotes;
            existingPayment.AppointmentId = payment.AppointmentId.HasValue ? payment.AppointmentId.Value : existingPayment.AppointmentId;
            existingPayment.PaymentMethodId = payment.PaymentMethodId.HasValue ? payment.PaymentMethodId.Value : existingPayment.PaymentMethodId;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments              
                .Where(p => p.PaymentId == paymentId)
                .Select(p => new Payment
                {
                   
                        PaymentId = p.PaymentId,
                        AmountPaid = p.AmountPaid,
                        PaymentDate = p.PaymentDate,
                        AdditionalNotes = p.AdditionalNotes,
                        AppointmentId = p.AppointmentId,
                        PaymentMethodId = p.PaymentMethodId
                    
                })
                .FirstOrDefaultAsync();
        }
        public async Task<Payment?> GetPaymentByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Payments
                .Where(p => p.AppointmentId == appointmentId)
                .Select(p => new Payment
                {
                  
                        PaymentId = p.PaymentId,
                        AmountPaid = p.AmountPaid,
                        PaymentDate = p.PaymentDate,
                        AdditionalNotes = p.AdditionalNotes,
                        AppointmentId = p.AppointmentId,
                        PaymentMethodId = p.PaymentMethodId
         
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
              
                .Select(p => new Payment
                {
                   
                        PaymentId = p.PaymentId,
                        AmountPaid = p.AmountPaid,
                        PaymentDate = p.PaymentDate,
                        AdditionalNotes = p.AdditionalNotes,
                        AppointmentId = p.AppointmentId,
                        PaymentMethodId = p.PaymentMethodId
                    
                })
                .ToListAsync();
        }
    }
}
