using ClinicAppManagerDataLayer.DataLayerInterfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppManagerDataLayer
{
    public class DoctorData : IDoctorData
    {
        private readonly AppDbContext _Context;
        private readonly IPersonData _PersonData;

        public DoctorData(AppDbContext context, IPersonData personData)
        {
            _Context = context ?? throw new ArgumentNullException(nameof(context));
            _PersonData = personData ?? throw new ArgumentNullException(nameof(personData));
        }

        public async Task<int?> AddNewDoctor(Entities.Doctor Doctor)
        {
            int? personId = await _PersonData.AddNewPerson(Doctor.Person);

            if (personId == null)
            {
                return null;
            }

            Doctor.PersonId = personId.Value;
            await _Context.Doctors.AddAsync(Doctor);
            await _Context.SaveChangesAsync();

            return Doctor.DoctorId;
        }

        public async Task<Entities.Doctor> GetDoctorInfoById(int DoctorId)
        {
            var doctor = await _Context.Doctors
                .FirstOrDefaultAsync(p => p.DoctorId == DoctorId);

            if (doctor != null)
            {
                doctor.Person = await _PersonData.GetPersonInfoById(doctor.PersonId);
            }

            return doctor;
        }

        public async Task<Entities.Doctor> GetDoctorInfoByFullName(string FullName)
        {
            var doctor = await _Context.Doctors
                .Include(p => p.Person)
                .FirstOrDefaultAsync(p => p.Person.FullName == FullName);

            if (doctor != null)
            {
                doctor.Person = await _PersonData.GetPersonInfoById(doctor.PersonId);
            }

            return doctor;
        }

        public async Task<Entities.Doctor> GetDoctorInfoByAspNetUserId(string aspNetUserId)
        {
            var doctor = await _Context.Doctors
                .FirstOrDefaultAsync(p => p.AspNetUserId == aspNetUserId);

            if (doctor != null)
            {
                doctor.Person = await _PersonData.GetPersonInfoById(doctor.PersonId);
            }

            return doctor;
        }

        public async Task<List<Entities.Doctor>> GetAllDoctors()
        {
            var doctors = await _Context.Doctors.ToListAsync();

            foreach (var doctor in doctors)
            {
                doctor.Person = await _PersonData.GetPersonInfoById(doctor.PersonId);
            }

            return doctors;
        }

        public async Task<bool> UpdateDoctorInfo(DTO.DoctorDTO Doctor)
        {
            var existingDoctor = await _Context.Doctors.FindAsync(Doctor.DoctorId);

            if (existingDoctor == null)
            {
                return false;
            }

            Doctor.Person.PersonId = existingDoctor.PersonId;

            if (!await _PersonData.UpdatePersonInfo(Doctor.Person))
            {
                return false;
            }

            existingDoctor.PhoneNumber = string.IsNullOrEmpty(Doctor.PhoneNumber)
                ? existingDoctor.PhoneNumber
                : Doctor.PhoneNumber;
            existingDoctor.IsActive = Doctor.IsActive ?? existingDoctor.IsActive;
            existingDoctor.Specialization = string.IsNullOrEmpty(Doctor.Specialization)
                ? existingDoctor.Specialization
                : Doctor.Specialization;

            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckDoctorExistByAspNetUserId(string aspNetUserId)
        {
            return await _Context.Doctors.AnyAsync(p => p.AspNetUserId == aspNetUserId);
        }
    }
}
