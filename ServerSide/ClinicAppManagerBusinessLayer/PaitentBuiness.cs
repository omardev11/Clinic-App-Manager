using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;

namespace ClinicAppManagerBusinessLayer
{
    public class PatientBuiness : IPatientBusiness
    {
        private readonly IPatientData _PatientData;
        public PatientDTO Patient { get; set; }
        public SecurityHelper.EnMode Mode;

        public PatientBuiness(IPatientData PatientData)
        {
            _PatientData = PatientData;
            Patient = new PatientDTO();
        }

        public void Initialize(PatientDTO Patient, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            this.Patient = Patient;
            Mode = mode;
        }

        private async Task<bool> _AddNewPatient()
        {
            if (Patient == null || string.IsNullOrWhiteSpace(Patient.AspNetUserId))
            {
                return false;
            }

            var patientEntity = new Patient
            {
                AspNetUserId = Patient.AspNetUserId,
                BloodType = Patient.BloodType,
                Person = new Person
                {
                    FirstName = Patient.Person.FirstName,
                    LastName = Patient.Person.LastName,
                    DateOfBirth = Patient.Person.DateOfBirth!.Value,
                    Address = Patient.Person.Address,
                    Gender = Patient.Person.Gender!.Value
                }
            };

            Patient.PatientId = await _PatientData.AddNewPatient(patientEntity);
            return Patient.PatientId.HasValue;
        }

        private async Task<bool> _UpdatePatient()
        {
            if (Patient == null || !Patient.PatientId.HasValue)
            {
                return false;
            }

            return await _PatientData.UpdatePatientInfo(Patient);
        }

        public async Task<PatientDTO> GetPatientInfoById(int PatientId)
        {
            if (PatientId <= 0)
            {
                return null;
            }

            var entityPatient = await _PatientData.GetPatientInfoById(PatientId);
            return MapToPatientDto(entityPatient);
        }

        public async Task<PatientDTO> GetPatientInfoByFullName(string FullName)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return null;
            }

            var entityPatient = await _PatientData.GetPatientInfoByFullName(FullName);
            return MapToPatientDto(entityPatient);
        }

        public async Task<PatientDTO> GetPatientInfoByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return null;
            }

            var entityPatient = await _PatientData.GetPatientInfoByAspNetUserId(aspNetUserId);
            return MapToPatientDto(entityPatient);
        }

        public async Task<List<PatientDTO>> GetAllPatients()
        {
            var entityPatients = await _PatientData.GetAllPatients();
            if (entityPatients == null || !entityPatients.Any())
            {
                return new List<PatientDTO>();
            }

            return entityPatients
                .Select(MapToPatientDto)
                .Where(patient => patient != null)
                .ToList();
        }

        public async Task<bool> CheckPatientExistByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return false;
            }

            return await _PatientData.CheckPatientExistByAspNetUserId(aspNetUserId);
        }

        public async Task<bool> CheckIfThisPatientHasThisDoctor(int patientId, int doctorId)
        {
            if (patientId <= 0 || doctorId <= 0)
            {
                return false;
            }

            return await _PatientData.CheckIfThisPatientHasThisDoctor(patientId, doctorId);
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case SecurityHelper.EnMode.AddNewMode:
                    return await _AddNewPatient();
                case SecurityHelper.EnMode.UpdateMode:
                    return await _UpdatePatient();
                default:
                    return false;
            }
        }

        private static PatientDTO MapToPatientDto(Patient entityPatient)
        {
            if (entityPatient == null)
            {
                return null;
            }

            return new PatientDTO
            {
                PatientId = entityPatient.PatientId,
                AspNetUserId = entityPatient.AspNetUserId,
                BloodType = entityPatient.BloodType,
                PersonId = entityPatient.PersonId,
                Person = new PersonDTO
                {
                    FirstName = entityPatient.Person.FirstName,
                    LastName = entityPatient.Person.LastName,
                    DateOfBirth = entityPatient.Person.DateOfBirth,
                    Address = entityPatient.Person.Address,
                    Age = entityPatient.Person.Age,
                    FullName = entityPatient.Person.FullName,
                    Gender = entityPatient.Person.Gender
                }
            };
        }
    }
}
