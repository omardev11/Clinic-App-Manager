using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using ClinicAppManagerDataLayer.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.Payments
{
    public class PaymentBusiness : IPaymentBusiness
    {
        private readonly IPaymentData _PaymentData;

        public PaymentDTO Payment { get;  set; }
        private readonly IPaymentMethodData _PaymentMethod;
        private SecurityHelper.EnMode Mode;

        public PaymentBusiness(IPaymentData paymentData , IPaymentMethodData paymentmethoddata)
        {
            _PaymentData = paymentData;
            _PaymentMethod = paymentmethoddata;
        }

        public void Initialize(PaymentDTO paymentdto, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            Payment = paymentdto;
            Mode = mode;

        }

        private async Task<bool> _AddNewPayment()
        {
            if (Payment == null || !Payment.AmountPaid.HasValue || !Payment.PaymentDate.HasValue ||
                !Payment.AppointmentId.HasValue || !Payment.PaymentMethodId.HasValue)
                return false;

            var entity = new Payment
            {
                AmountPaid = Payment.AmountPaid.Value,
                PaymentDate = Payment.PaymentDate.Value,
                AdditionalNotes = Payment.AdditionalNotes,
                AppointmentId = Payment.AppointmentId.Value,
                PaymentMethodId = Payment.PaymentMethodId.Value
            };

            Payment.PaymentId = await _PaymentData.AddPaymentAsync(entity);
            return Payment.PaymentId.HasValue;
        }

        private async Task<bool> _UpdatePayment()
        {
            if (!Payment.PaymentId.HasValue)
                return false;

            return await _PaymentData.UpdatePaymentAsync(Payment);
        }

        public async Task<DetailPaymentDTO?> GetPaymentById(int PaymentId)
        {
            var entity = await _PaymentData.GetPaymentByIdAsync(PaymentId);
            if (entity == null)
                return null;

            return new DetailPaymentDTO
            {
                PaymentDTO = new PaymentDTO
                {
                    PaymentId = entity.PaymentId,
                    AmountPaid = entity.AmountPaid,
                    PaymentDate = entity.PaymentDate,
                    AdditionalNotes = entity.AdditionalNotes,
                    AppointmentId = entity.AppointmentId,
                    PaymentMethodId = entity.PaymentMethodId
                },
                PaymentMethodName = (await _PaymentMethod.GetPaymentMethodById(entity.PaymentMethodId)).MethodName
            };
        }
        public async Task<DetailPaymentDTO?> GetPaymentByAppointmentId(int AppointementId)
        {
            var entity = await _PaymentData.GetPaymentByAppointmentIdAsync(AppointementId);
            if (entity == null)
                return null;

            return new DetailPaymentDTO
            {
                PaymentDTO = new PaymentDTO
                {
                    PaymentId = entity.PaymentId,
                    AmountPaid = entity.AmountPaid,
                    PaymentDate = entity.PaymentDate,
                    AdditionalNotes = entity.AdditionalNotes,
                    AppointmentId = entity.AppointmentId,
                    PaymentMethodId = entity.PaymentMethodId
                },
                PaymentMethodName = (await _PaymentMethod.GetPaymentMethodById(entity.PaymentMethodId)).MethodName
            };
        }


        public async Task<List<DetailPaymentDTO>> GetAllPayments()
        {
            var payments = await _PaymentData.GetAllPaymentsAsync();

          
            var mappingTasks = payments.Select(async x =>
            {
                var paymentMethod = await _PaymentMethod.GetPaymentMethodById(x.PaymentMethodId);

                return new DetailPaymentDTO
                {
                    PaymentDTO = new PaymentDTO
                    {
                        PaymentId = x.PaymentId,
                        AmountPaid = x.AmountPaid,
                        PaymentDate = x.PaymentDate,
                        AdditionalNotes = x.AdditionalNotes,
                        AppointmentId = x.AppointmentId,
                        PaymentMethodId = x.PaymentMethodId
                    },
                    PaymentMethodName = paymentMethod.MethodName
                };
            }).ToList(); // .ToList() here is crucial to start the tasks immediately

         
            var detailedPayments = await Task.WhenAll(mappingTasks);

            // 4. Convert the resulting array back to a List and return.
            return detailedPayments.ToList();
        }

        public async Task<bool> Save()
        {
            return Mode switch
            {
                SecurityHelper.EnMode.AddNewMode => await _AddNewPayment(),
                SecurityHelper.EnMode.UpdateMode => await _UpdatePayment(),
                _ => false
            };
        }
    }
}

