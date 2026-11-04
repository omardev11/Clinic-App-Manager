using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IMedicalRecordBusiness
    {
        public Task<MedicalRecordDTO> GetMedicalRecordById(int recordId);
        public Task<List<MedicalRecordDTO>> GetMedicalRecordsByAppointmentId(int appointmentId);
        public Task<List<MedicalRecordDTO>> GetAllMedicalRecords();
        public Task<bool> Save();
        public MedicalRecordDTO MedicalRecord { get; set; }

        public void Initialize(MedicalRecordDTO medicalRecord, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
    }
}
