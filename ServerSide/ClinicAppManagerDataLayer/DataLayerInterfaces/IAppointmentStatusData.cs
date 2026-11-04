using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IAppointmentStatusData
    {
        public Task<int?> AddNewAppointmentStatus(Entities.AppointmentStatus AppointmentStatus);
        public Task<Entities.AppointmentStatus> GetAppointmentStatusById(int AppointmentStatusId);
        public Task<Entities.AppointmentStatus> GetAppointmentStatusByStatusName(string StatusName);
        public Task<List<Entities.AppointmentStatus>> GetAllAppointmentStatuses();
        public Task<bool> UpdateAppointmentStatus(DTO.AppointmentStatusDTO AppointmentStatus);
        public Task<bool> DeleteAppointmentStatus(int AppointmentStatusId);
    }
}
