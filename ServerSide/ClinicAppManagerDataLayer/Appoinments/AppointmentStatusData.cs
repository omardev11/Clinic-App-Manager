using ClinicAppManagerDataLayer.DataLayerInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Appoinments
{
    public class AppointmentStatusData : IAppointmentStatusData
    {

        private readonly AppDbContext _context;

        public AppointmentStatusData(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<int?> AddNewAppointmentStatus(Entities.AppointmentStatus AppointmentStatus)
        {

            await _context.AppointmentStatuses.AddAsync(AppointmentStatus);
            await _context.SaveChangesAsync();

            return AppointmentStatus.AppointmentStatusId;
        }
        public async Task<Entities.AppointmentStatus> GetAppointmentStatusById(int AppointmentStatusId)
        {
            var AppointmentStatus = await _context.AppointmentStatuses
            .Where(p => p.AppointmentStatusId == AppointmentStatusId)
            .FirstOrDefaultAsync();

            return AppointmentStatus;
        }
        public async Task<Entities.AppointmentStatus> GetAppointmentStatusByStatusName(string StatusName)
        {
            var AppointmentStatus = await _context.AppointmentStatuses
            .Where(p => p.StatusName == StatusName)
            .FirstOrDefaultAsync();

            return AppointmentStatus;
        }
        public async Task<List<Entities.AppointmentStatus>> GetAllAppointmentStatuses()
        {
            return  await _context.AppointmentStatuses.ToListAsync();

        }
        public async Task<bool> DeleteAppointmentStatus(int AppointmentStatusId)
        {
            var AppointmentStatus = await _context.AppointmentStatuses.FindAsync(AppointmentStatusId);
            if (AppointmentStatus == null)
                return false; // AppointmentStatus not found, return false
            _context.AppointmentStatuses.Remove(AppointmentStatus);
            await _context.SaveChangesAsync();
            return true; // Deletion successful
        }
        public async Task<bool> UpdateAppointmentStatus(DTO.AppointmentStatusDTO AppointmentStatus)
        {
          
            var ExistingAppointmentStatus = await _context.AppointmentStatuses.FindAsync(AppointmentStatus.AppointmentStatusId);
            if (ExistingAppointmentStatus == null)
                return false; // AppointmentStatus not found, return false


            ExistingAppointmentStatus.StatusName = AppointmentStatus.StatusName;
          
            await _context.SaveChangesAsync();
            return true; // Update successful
        }


    }
}
