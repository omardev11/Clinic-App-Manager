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
    public class PaymentMethodData : IPaymentMethodData
    {
        private readonly AppDbContext _context;

        public PaymentMethodData(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int?> AddNewPaymentMethod(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Add(paymentMethod);
            await _context.SaveChangesAsync();
            return paymentMethod.PaymentMethodId;
        }

        public async Task<bool> UpdatePaymentMethod(PaymentMethodDTO paymentMethod)
        {
            var existing = await _context.PaymentMethods.FindAsync(paymentMethod.PaymentMethodId);
            if (existing == null)
                return false;

            existing.MethodName = paymentMethod.PaymentMethodName;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePaymentMethod(int PaymentMethodId)
        {
            var existing = await _context.PaymentMethods.FindAsync(PaymentMethodId);
            if (existing == null)
                return false;

            _context.PaymentMethods.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentMethod?> GetPaymentMethodById(int PaymentMethodId)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PaymentMethodId == PaymentMethodId);
        }

        public async Task<PaymentMethod?> GetPaymentMethodByName(string MethodName)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.MethodName == MethodName);
        }

        public async Task<List<PaymentMethod>> GetAllPaymentMethods()
        {
            return await _context.PaymentMethods.AsNoTracking().ToListAsync();
        }
    }
}
