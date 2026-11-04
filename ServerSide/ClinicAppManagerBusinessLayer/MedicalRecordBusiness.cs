using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer
{
    public class MedicalRecordBusiness : IMedicalRecordBusiness
    {
        private readonly IMedicalRecordData _medicalRecordData;
        public MedicalRecordDTO MedicalRecord { get; set; }
        private SecurityHelper.EnMode Mode;

        public MedicalRecordBusiness(IMedicalRecordData medicalRecordData)
        {
            _medicalRecordData = medicalRecordData;
            MedicalRecord = new MedicalRecordDTO();
        }

        public void Initialize(MedicalRecordDTO medicalRecord, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            this.MedicalRecord = medicalRecord;
            this.Mode = mode;
        }

        // -------------------------------------------------------------------------
        // ADD NEW MEDICAL RECORD
        private async Task<bool> _AddNewMedicalRecord()
        {
            if (MedicalRecord == null)
                return false;

            var entity = new MedicalRecord
            {
                VisitDescription = MedicalRecord.VisitDescription,
                Diagnosis = MedicalRecord.Diagnosis,
                AdditionalNotes = MedicalRecord.AdditionalNotes,
                AppoinmentId = MedicalRecord.AppoinmentId.Value
            };

            MedicalRecord.RecordId = await _medicalRecordData.AddNewMedicalRecord(entity);
            return MedicalRecord.RecordId.HasValue;
        }

        // -------------------------------------------------------------------------
        // UPDATE MEDICAL RECORD
        private async Task<bool> _UpdateMedicalRecord()
        {
            if (!MedicalRecord.RecordId.HasValue)
                return false;

            return await _medicalRecordData.UpdateMedicalRecord(MedicalRecord);
        }

        // -------------------------------------------------------------------------
        // DELETE MEDICAL RECORD
        private async Task<bool> _DeleteMedicalRecord()
        {
            if (!MedicalRecord.RecordId.HasValue)
                return false;

            return await _medicalRecordData.DeleteMedicalRecord(MedicalRecord.RecordId.Value);
        }

        // -------------------------------------------------------------------------
        // GET RECORD BY ID
        public async Task<MedicalRecordDTO> GetMedicalRecordById(int recordId)
        {
            if (recordId <= 0)
                return null;

            var entity = await _medicalRecordData.GetMedicalRecordById(recordId);
            if (entity == null)
                return null;

            return new MedicalRecordDTO
            {
                RecordId = entity.RecordId,
                VisitDescription = entity.VisitDescription,
                Diagnosis = entity.Diagnosis,
                AdditionalNotes = entity.AdditionalNotes,
                AppoinmentId = entity.AppoinmentId
            };
        }

        // -------------------------------------------------------------------------
        // GET RECORDS BY APPOINTMENT ID
        public async Task<List<MedicalRecordDTO>> GetMedicalRecordsByAppointmentId(int appointmentId)
        {
            if (appointmentId <= 0)
                return new List<MedicalRecordDTO>();

            var entities = await _medicalRecordData.GetMedicalRecordsByAppointmentId(appointmentId);
            if (!entities.Any())
                return new List<MedicalRecordDTO>();

            return entities.Select(entity => new MedicalRecordDTO
            {
                RecordId = entity.RecordId,
                VisitDescription = entity.VisitDescription,
                Diagnosis = entity.Diagnosis,
                AdditionalNotes = entity.AdditionalNotes,
                AppoinmentId = entity.AppoinmentId
            }).ToList();
        }

        // -------------------------------------------------------------------------
        // GET ALL RECORDS
        public async Task<List<MedicalRecordDTO>> GetAllMedicalRecords()
        {
            var entities = await _medicalRecordData.GetAllMedicalRecords();
            if (!entities.Any())
                return new List<MedicalRecordDTO>();

            return entities.Select(entity => new MedicalRecordDTO
            {
                RecordId = entity.RecordId,
                VisitDescription = entity.VisitDescription,
                Diagnosis = entity.Diagnosis,
                AdditionalNotes = entity.AdditionalNotes,
                AppoinmentId = entity.AppoinmentId
            }).ToList();
        }

        // -------------------------------------------------------------------------
        // SAVE (Add, Update, Delete)
        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case SecurityHelper.EnMode.AddNewMode:
                    return await _AddNewMedicalRecord();
                case SecurityHelper.EnMode.UpdateMode:
                    return await _UpdateMedicalRecord();
                case SecurityHelper.EnMode.DeleteMode:
                    return await _DeleteMedicalRecord();
                default:
                    return false;
            }
        }
    }
}
