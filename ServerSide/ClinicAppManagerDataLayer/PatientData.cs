using ClinicAppManagerDataLayer.DataLayerInterfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppManagerDataLayer
{
    public class PatientData : IPatientData
    {
        private readonly AppDbContext _Context;
        private readonly IPersonData _PersonData;

        public PatientData(AppDbContext context, IPersonData personData)
        {
            _Context = context ?? throw new ArgumentNullException(nameof(context));
            _PersonData = personData ?? throw new ArgumentNullException(nameof(personData));
        }

        public async Task<int?> AddNewPatient(Entities.Patient Patient)
        {
            int? personId = await _PersonData.AddNewPerson(Patient.Person);

            if (personId == null)
            {
                return null;
            }

            Patient.PersonId = personId.Value;
            await _Context.Patients.AddAsync(Patient);
            await _Context.SaveChangesAsync();

            return Patient.PatientId;
        }

        public async Task<Entities.Patient> GetPatientInfoById(int PatientId)
        {
            var patient = await _Context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == PatientId);

            if (patient != null)
            {
                patient.Person = await _PersonData.GetPersonInfoById(patient.PersonId);
            }

            return patient;
        }

        public async Task<Entities.Patient> GetPatientInfoByFullName(string FullName)
        {
            var patient = await _Context.Patients
                .Include(p => p.Person)
                .FirstOrDefaultAsync(p => p.Person.FullName == FullName);

            if (patient != null)
            {
                patient.Person = await _PersonData.GetPersonInfoById(patient.PersonId);
            }

            return patient;
        }

        public async Task<Entities.Patient> GetPatientInfoByAspNetUserId(string aspNetUserId)
        {
            var patient = await _Context.Patients
                .FirstOrDefaultAsync(p => p.AspNetUserId == aspNetUserId);

            if (patient != null)
            {
                patient.Person = await _PersonData.GetPersonInfoById(patient.PersonId);
            }

            return patient;
        }

        public async Task<List<Entities.Patient>> GetAllPatients()
        {
            var patients = await _Context.Patients.ToListAsync();

            foreach (var patient in patients)
            {
                patient.Person = await _PersonData.GetPersonInfoById(patient.PersonId);
            }

            return patients;
        }

        public async Task<bool> UpdatePatientInfo(DTO.PatientDTO Patient)
        {
            var existingPatient = await _Context.Patients.FindAsync(Patient.PatientId);

            if (existingPatient == null)
            {
                return false;
            }

            Patient.Person.PersonId = existingPatient.PersonId;

            if (!await _PersonData.UpdatePersonInfo(Patient.Person))
            {
                return false;
            }

            existingPatient.BloodType = string.IsNullOrEmpty(Patient.BloodType)
                ? existingPatient.BloodType
                : Patient.BloodType;

            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckPatientExistByAspNetUserId(string aspNetUserId)
        {
            return await _Context.Patients.AnyAsync(p => p.AspNetUserId == aspNetUserId);
        }
        public async Task<bool> CheckIfThisPatientHasThisDoctor(int patientId,int DoctorId)
        {
            return await _Context.Appointments.AnyAsync(p => p.PatientId == patientId && p.DoctorId == DoctorId
                                                                           && p.AppointmentStatusId == 2);
        }
    }
}
