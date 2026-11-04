using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IMedicalRecordData
    {
        Task<int?> AddNewMedicalRecord(MedicalRecord record);
        Task<MedicalRecord> GetMedicalRecordById(int recordId);
        Task<List<MedicalRecord>> GetMedicalRecordsByAppointmentId(int appointmentId);
        Task<List<MedicalRecord>> GetAllMedicalRecords();
        Task<bool> UpdateMedicalRecord(DTO.MedicalRecordDTO record);
        Task<bool> DeleteMedicalRecord(int recordId);
    }
}
