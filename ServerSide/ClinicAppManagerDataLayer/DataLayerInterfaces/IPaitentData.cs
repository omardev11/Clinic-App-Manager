using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IPatientData
    {
        public Task<int?> AddNewPatient(Entities.Patient Patient);
        public Task<Entities.Patient> GetPatientInfoById(int PatientId);
        public Task<Entities.Patient> GetPatientInfoByFullName(string FullName);
        public Task<Entities.Patient> GetPatientInfoByAspNetUserId(string aspNetUserId);
        public Task<List<Entities.Patient>> GetAllPatients();
        public Task<bool> UpdatePatientInfo(DTO.PatientDTO Patient);
        public Task<bool> CheckPatientExistByAspNetUserId(string aspNetUserId);
        public Task<bool> CheckIfThisPatientHasThisDoctor(int patientId, int doctorId);
    }
}
