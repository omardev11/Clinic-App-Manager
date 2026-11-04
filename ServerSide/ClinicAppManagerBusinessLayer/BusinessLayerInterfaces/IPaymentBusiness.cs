using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IPaymentBusiness
    {
        PaymentDTO Payment { get; set; }
        void Initialize(PaymentDTO payment, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        Task<bool> Save();
        Task<DetailPaymentDTO?> GetPaymentById(int PaymentId);
        Task<DetailPaymentDTO?> GetPaymentByAppointmentId(int AppointmentId);

        Task<List<DetailPaymentDTO>> GetAllPayments();
    }
}
