using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IAppointmentStatusBusiness
    {
        public Task<AppointmentStatusDTO> GetAppointmentStatusById(int AppointmentStatusId);
        public Task<AppointmentStatusDTO> GetAppointmentStatusByStatusName(string StatusName);
        public Task<List<AppointmentStatusDTO>> GetAllAppointmentStatuses();
        public void Initialize(AppointmentStatusDTO AppointmentStatus, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        public AppointmentStatusDTO AppointmentStatus { get; set; }
        public Task<bool> Save();
    }
}
