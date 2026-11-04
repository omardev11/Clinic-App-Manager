using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IPaymentMethodData
    {
        Task<int?> AddNewPaymentMethod(PaymentMethod paymentMethod);
        Task<bool> UpdatePaymentMethod(PaymentMethodDTO paymentMethod);
        Task<bool> DeletePaymentMethod(int PaymentMethodId);
        Task<PaymentMethod?> GetPaymentMethodById(int PaymentMethodId);
        Task<PaymentMethod?> GetPaymentMethodByName(string MethodName);
        Task<List<PaymentMethod>> GetAllPaymentMethods();
    }
}
