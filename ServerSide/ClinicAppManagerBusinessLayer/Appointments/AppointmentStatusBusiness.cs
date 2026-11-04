using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.Appointments
{
    public class AppointmentStatusBusiness : IAppointmentStatusBusiness
    {
        private readonly IAppointmentStatusData _AppointmentStatusData; 
        public AppointmentStatusDTO AppointmentStatus { get; set; }
        private SecurityHelper.EnMode Mode;
        public AppointmentStatusBusiness(IAppointmentStatusData AppointmentStatusData)
        {
            _AppointmentStatusData = AppointmentStatusData;
            AppointmentStatus = new AppointmentStatusDTO();
        }

        public void Initialize(AppointmentStatusDTO AppointmentStatus, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            this.AppointmentStatus = AppointmentStatus;
            this.Mode = mode;
        }

        private async Task<bool> _AddNewAppointmentStatus()
        {
            if (AppointmentStatus == null)
                return false;

            var AppointmentStatusEntity = new AppointmentStatus
            {
                StatusName = AppointmentStatus.StatusName
            };

            AppointmentStatus.AppointmentStatusId = await _AppointmentStatusData.AddNewAppointmentStatus(AppointmentStatusEntity);
            return AppointmentStatus.AppointmentStatusId.HasValue;
        }
        private async Task<bool> _UpdateAppointmentStatus()
        {
            if (string.IsNullOrWhiteSpace(AppointmentStatus.StatusName) || !AppointmentStatus.AppointmentStatusId.HasValue)
                return false;
          
            return await _AppointmentStatusData.UpdateAppointmentStatus(AppointmentStatus);
        }
        private async Task<bool> _DeleteAppointmentStatus()
        {
            if (!AppointmentStatus.AppointmentStatusId.HasValue)
                return false;

            return await _AppointmentStatusData.DeleteAppointmentStatus(AppointmentStatus.AppointmentStatusId.Value);
        }

        public async Task<AppointmentStatusDTO> GetAppointmentStatusById(int AppointmentStatusId)
        {
            if (AppointmentStatusId <= 0)
                return null;

            var EntityAppointmentStatus = await _AppointmentStatusData.GetAppointmentStatusById(AppointmentStatusId);
            if (EntityAppointmentStatus == null)
                return null;

            // Mapping Entity to DTO
            var AppointmentStatusDto = new AppointmentStatusDTO
            {
                AppointmentStatusId = EntityAppointmentStatus.AppointmentStatusId,
                StatusName = EntityAppointmentStatus.StatusName
                
            };
            return AppointmentStatusDto;
        }
        public async Task<AppointmentStatusDTO> GetAppointmentStatusByStatusName(string StatusName)
        {
            if (string.IsNullOrWhiteSpace(StatusName))
                return null;

            var EntityAppointmentStatus = await _AppointmentStatusData.GetAppointmentStatusByStatusName(StatusName);
            if (EntityAppointmentStatus == null)
                return null;
            // Mapping Entity to DTO
            var AppointmentStatusDto = new AppointmentStatusDTO
            {
                AppointmentStatusId = EntityAppointmentStatus.AppointmentStatusId,
                StatusName = EntityAppointmentStatus.StatusName
            };
            return AppointmentStatusDto;
        }
        public async Task<List<AppointmentStatusDTO>> GetAllAppointmentStatuses()
        {
            var EntityAppointmentStatuss = await _AppointmentStatusData.GetAllAppointmentStatuses();
            if (!EntityAppointmentStatuss.Any())
                return new List<AppointmentStatusDTO>();
            // Mapping Entities to DTOs
            var AppointmentStatusDtos = EntityAppointmentStatuss.Select(EntityAppointmentStatus => new AppointmentStatusDTO
            {
                AppointmentStatusId = EntityAppointmentStatus.AppointmentStatusId,
                StatusName=EntityAppointmentStatus.StatusName
            }).ToList();

            return AppointmentStatusDtos;
        }
        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case SecurityHelper.EnMode.AddNewMode:
                    return await _AddNewAppointmentStatus();
                case SecurityHelper.EnMode.UpdateMode:
                    return await _UpdateAppointmentStatus();
                case SecurityHelper.EnMode.DeleteMode:
                    return await _DeleteAppointmentStatus();
                default:
                    return false;
            }
        }
    }
}
