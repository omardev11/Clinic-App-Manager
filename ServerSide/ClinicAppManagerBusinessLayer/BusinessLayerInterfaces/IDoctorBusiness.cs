using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClinicAppManagerBusinessLayer.DoctorBuiness;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IDoctorBusiness
    {
        public Task<DoctorDTO> GetDoctorInfoById(int DoctorId);
        public Task<bool> CheckDoctorExistByAspNetUserId(string aspNetUserId);
        public Task<DoctorDTO> GetDoctorInfoByFullName(string FullName);
        public Task<DoctorDTO> GetDoctorInfoByAspNetUserId(string aspNetUserId);
        public Task<List<DoctorDTO>> GetAllDoctors();
        public void Initialize(DoctorDTO Doctor, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        public DoctorDTO Doctor { get; set; }

        public Task<bool> Save();
    }
}
