
using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IPaymentMethodBusiness
    {
        PaymentMethodDTO PaymentMethod { get; set; }
        void Initialize(PaymentMethodDTO paymentMethod, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        Task<bool> Save();
        Task<PaymentMethodDTO?> GetPaymentMethodById(int PaymentMethodId);
        Task<PaymentMethodDTO?> GetPaymentMethodByName(string MethodName);
        Task<List<PaymentMethodDTO>> GetAllPaymentMethods();
    }
}
