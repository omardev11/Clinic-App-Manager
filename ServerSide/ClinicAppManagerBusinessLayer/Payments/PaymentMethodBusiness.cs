using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.Payments
{
    public class PaymentMethodBusiness : IPaymentMethodBusiness
    {
        private readonly IPaymentMethodData _PaymentMethodData;
        public PaymentMethodDTO PaymentMethod { get; set; }
        private SecurityHelper.EnMode Mode;

        public PaymentMethodBusiness(IPaymentMethodData paymentMethodData)
        {
            _PaymentMethodData = paymentMethodData;
            PaymentMethod = new PaymentMethodDTO();
        }

        public void Initialize(PaymentMethodDTO paymentMethod, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            this.PaymentMethod = paymentMethod;
            this.Mode = mode;
        }

        private async Task<bool> _AddNewPaymentMethod()
        {
            if (PaymentMethod == null || string.IsNullOrWhiteSpace(PaymentMethod.PaymentMethodName))
                return false;

            var entity = new PaymentMethod
            {
                MethodName = PaymentMethod.PaymentMethodName
            };

            PaymentMethod.PaymentMethodId = await _PaymentMethodData.AddNewPaymentMethod(entity);
            return PaymentMethod.PaymentMethodId.HasValue;
        }

        private async Task<bool> _UpdatePaymentMethod()
        {
            if (!PaymentMethod.PaymentMethodId.HasValue)
                return false;

            return await _PaymentMethodData.UpdatePaymentMethod(PaymentMethod);
        }

        private async Task<bool> _DeletePaymentMethod()
        {
            if (!PaymentMethod.PaymentMethodId.HasValue)
                return false;

            return await _PaymentMethodData.DeletePaymentMethod(PaymentMethod.PaymentMethodId.Value);
        }

        public async Task<PaymentMethodDTO?> GetPaymentMethodById(int PaymentMethodId)
        {
            var entity = await _PaymentMethodData.GetPaymentMethodById(PaymentMethodId);
            if (entity == null)
                return null;

            return new PaymentMethodDTO
            {
                PaymentMethodId = entity.PaymentMethodId,
                PaymentMethodName = entity.MethodName
            };
        }

        public async Task<PaymentMethodDTO?> GetPaymentMethodByName(string PaymentMethodName)
        {
            var entity = await _PaymentMethodData.GetPaymentMethodByName(PaymentMethodName);
            if (entity == null)
                return null;

            return new PaymentMethodDTO
            {
                PaymentMethodId = entity.PaymentMethodId,
                PaymentMethodName = entity.MethodName
            };
        }

        public async Task<List<PaymentMethodDTO>> GetAllPaymentMethods()
        {
            var list = await _PaymentMethodData.GetAllPaymentMethods();
            return list.Select(x => new PaymentMethodDTO
            {
                PaymentMethodId = x.PaymentMethodId,
                PaymentMethodName = x.MethodName
            }).ToList();
        }

        public async Task<bool> Save()
        {
            return Mode switch
            {
                SecurityHelper.EnMode.AddNewMode => await _AddNewPaymentMethod(),
                SecurityHelper.EnMode.UpdateMode => await _UpdatePaymentMethod(),
                SecurityHelper.EnMode.DeleteMode => await _DeletePaymentMethod(),
                _ => false
            };
        }
    }
}
