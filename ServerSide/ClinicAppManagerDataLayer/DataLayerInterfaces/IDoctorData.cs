using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IDoctorData
    {
        public Task<int?> AddNewDoctor(Entities.Doctor Doctor);
        public Task<Entities.Doctor> GetDoctorInfoById(int DoctorId);
        public Task<Entities.Doctor> GetDoctorInfoByFullName(string FullName);
        public Task<Entities.Doctor> GetDoctorInfoByAspNetUserId(string aspNetUserId);
        public Task<List<Entities.Doctor>> GetAllDoctors();
        public Task<bool> UpdateDoctorInfo(DTO.DoctorDTO Doctor);
        public Task<bool> CheckDoctorExistByAspNetUserId(string aspNetUserId);
    }
}
