using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;

namespace ClinicAppManagerBusinessLayer
{
    public class DoctorBuiness : IDoctorBusiness
    {
        private readonly IDoctorData _DoctorData;
        public DoctorDTO Doctor { get; set; }
        public SecurityHelper.EnMode Mode;

        public DoctorBuiness(IDoctorData DoctorData)
        {
            _DoctorData = DoctorData;
            Doctor = new DoctorDTO();
        }

        public void Initialize(DoctorDTO Doctor, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            this.Doctor = Doctor;
            Mode = mode;
        }

        private async Task<bool> _AddNewDoctor()
        {
            if (Doctor == null || string.IsNullOrWhiteSpace(Doctor.AspNetUserId))
            {
                return false;
            }

            var doctorEntity = new Doctor
            {
                AspNetUserId = Doctor.AspNetUserId,
                Specialization = Doctor.Specialization,
                PhoneNumber = Doctor.PhoneNumber,
                IsActive = Doctor.IsActive ?? true,
                Person = new Person
                {
                    FirstName = Doctor.Person.FirstName,
                    LastName = Doctor.Person.LastName,
                    DateOfBirth = Doctor.Person.DateOfBirth!.Value,
                    Address = Doctor.Person.Address,
                    Gender = Doctor.Person.Gender!.Value
                }
            };

            Doctor.DoctorId = await _DoctorData.AddNewDoctor(doctorEntity);
            return Doctor.DoctorId.HasValue;
        }

        private async Task<bool> _UpdateDoctor()
        {
            if (Doctor == null || !Doctor.DoctorId.HasValue)
            {
                return false;
            }

            return await _DoctorData.UpdateDoctorInfo(Doctor);
        }

        public async Task<DoctorDTO> GetDoctorInfoById(int DoctorId)
        {
            if (DoctorId <= 0)
            {
                return null;
            }

            var entityDoctor = await _DoctorData.GetDoctorInfoById(DoctorId);
            return MapToDoctorDto(entityDoctor);
        }

        public async Task<DoctorDTO> GetDoctorInfoByFullName(string FullName)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return null;
            }

            var entityDoctor = await _DoctorData.GetDoctorInfoByFullName(FullName);
            return MapToDoctorDto(entityDoctor);
        }

        public async Task<DoctorDTO> GetDoctorInfoByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return null;
            }

            var entityDoctor = await _DoctorData.GetDoctorInfoByAspNetUserId(aspNetUserId);
            return MapToDoctorDto(entityDoctor);
        }

        public async Task<List<DoctorDTO>> GetAllDoctors()
        {
            var entityDoctors = await _DoctorData.GetAllDoctors();
            if (entityDoctors == null || !entityDoctors.Any())
            {
                return new List<DoctorDTO>();
            }

            return entityDoctors
                .Select(MapToDoctorDto)
                .Where(doctor => doctor != null)
                .ToList();
        }

        public async Task<bool> CheckDoctorExistByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return false;
            }

            return await _DoctorData.CheckDoctorExistByAspNetUserId(aspNetUserId);
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case SecurityHelper.EnMode.AddNewMode:
                    return await _AddNewDoctor();
                case SecurityHelper.EnMode.UpdateMode:
                    return await _UpdateDoctor();
                default:
                    return false;
            }
        }

        private static DoctorDTO MapToDoctorDto(Doctor entityDoctor)
        {
            if (entityDoctor == null)
            {
                return null;
            }

            return new DoctorDTO
            {
                DoctorId = entityDoctor.DoctorId,
                AspNetUserId = entityDoctor.AspNetUserId,
                Specialization = entityDoctor.Specialization,
                PhoneNumber = entityDoctor.PhoneNumber,
                IsActive = entityDoctor.IsActive,
                PersonId = entityDoctor.PersonId,
                Person = new PersonDTO
                {
                    FirstName = entityDoctor.Person.FirstName,
                    LastName = entityDoctor.Person.LastName,
                    DateOfBirth = entityDoctor.Person.DateOfBirth,
                    Address = entityDoctor.Person.Address,
                    Age = entityDoctor.Person.Age,
                    FullName = entityDoctor.Person.FullName,
                    Gender = entityDoctor.Person.Gender
                }
            };
        }
    }
}
