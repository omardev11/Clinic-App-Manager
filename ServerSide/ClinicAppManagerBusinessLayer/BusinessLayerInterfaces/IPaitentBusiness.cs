using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClinicAppManagerBusinessLayer.PatientBuiness;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IPatientBusiness
    {
        public Task<PatientDTO> GetPatientInfoById(int PatientId);
        public Task<bool> CheckPatientExistByAspNetUserId(string aspNetUserId);
        public Task<PatientDTO> GetPatientInfoByFullName(string FullName);
        public Task<PatientDTO> GetPatientInfoByAspNetUserId(string aspNetUserId);
        public Task<List<PatientDTO>> GetAllPatients();
        public Task<bool> CheckIfThisPatientHasThisDoctor(int patientId, int doctorId);
        public void Initialize(PatientDTO Patient, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        public PatientDTO Patient { get; set; }

        public Task<bool> Save();
    }
}
