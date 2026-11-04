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
    public class PrescribedMedicationBusiness : IPrescribedMedicationBusiness
    {
        private readonly IPrescribedMedicationData _PrescribedMedicationData;
        public PrescribedMedicationDTO PrescribedMedication { get; set; }
        private SecurityHelper.EnMode Mode;

        public PrescribedMedicationBusiness(IPrescribedMedicationData prescribedMedicationData)
        {
            _PrescribedMedicationData = prescribedMedicationData;
        }

        public void Initialize(PrescribedMedicationDTO prescribedMedicationDto, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            PrescribedMedication = prescribedMedicationDto;
            Mode = mode;
        }

        private async Task<bool> _AddNewPrescribedMedication()
        {
            if (PrescribedMedication == null ||
                string.IsNullOrWhiteSpace(PrescribedMedication.MedicatoinName) ||
                string.IsNullOrWhiteSpace(PrescribedMedication.Dosage) ||
                string.IsNullOrWhiteSpace(PrescribedMedication.Frequency) ||
                !PrescribedMedication.StartDate.HasValue ||
                !PrescribedMedication.EndDate.HasValue ||
                !PrescribedMedication.RecordId.HasValue)
                return false;

            var entity = new PrescribedMedication
            {
                MedicatoinName = PrescribedMedication.MedicatoinName,
                Dosage = PrescribedMedication.Dosage,
                Frequency = PrescribedMedication.Frequency,
                StartDate = PrescribedMedication.StartDate.Value,
                EndDate = PrescribedMedication.EndDate.Value,
                SpecialInstructions = PrescribedMedication.SpecialInstructions,
                RecordId = PrescribedMedication.RecordId.Value
            };

            PrescribedMedication.PrescriptionId = await _PrescribedMedicationData.AddPrescribedMedicationAsync(entity);
            return PrescribedMedication.PrescriptionId.HasValue;
        }

        private async Task<bool> _UpdatePrescribedMedication()
        {
            if (!PrescribedMedication.PrescriptionId.HasValue)
                return false;

            return await _PrescribedMedicationData.UpdatePrescribedMedicationAsync(PrescribedMedication);
        }

        private async Task<bool> _DeletePrescribedMedication()
        {
            if (!PrescribedMedication.PrescriptionId.HasValue)
                return false;

            return await _PrescribedMedicationData.DeletePrescribedMedicationAsync(PrescribedMedication.PrescriptionId.Value);
        }

        public async Task<PrescribedMedicationDTO?> GetPrescribedMedicationById(int prescriptionId)
        {
            var entity = await _PrescribedMedicationData.GetPrescribedMedicationByIdAsync(prescriptionId);
            if (entity == null)
                return null;

            return new PrescribedMedicationDTO
            {
                PrescriptionId = entity.PrescriptionId,
                MedicatoinName = entity.MedicatoinName,
                Dosage = entity.Dosage,
                Frequency = entity.Frequency,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                SpecialInstructions = entity.SpecialInstructions,
                RecordId = entity.RecordId
            };
        }

        public async Task<PrescribedMedicationDTO?> GetPrescribedMedicationByRecordId(int recordId)
        {
            var entity = await _PrescribedMedicationData.GetPrescribedMedicationByRecordIdAsync(recordId);
            if (entity == null)
                return null;

            return new PrescribedMedicationDTO
            {
                PrescriptionId = entity.PrescriptionId,
                MedicatoinName = entity.MedicatoinName,
                Dosage = entity.Dosage,
                Frequency = entity.Frequency,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                SpecialInstructions = entity.SpecialInstructions,
                RecordId = entity.RecordId
            };
        }

        public async Task<List<PrescribedMedicationDTO>> GetAllPrescribedMedications()
        {
            var list = await _PrescribedMedicationData.GetAllPrescribedMedicationsAsync();

            return list.Select(x => new PrescribedMedicationDTO
            {
                PrescriptionId = x.PrescriptionId,
                MedicatoinName = x.MedicatoinName,
                Dosage = x.Dosage,
                Frequency = x.Frequency,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                SpecialInstructions = x.SpecialInstructions,
                RecordId = x.RecordId
            }).ToList();
        }

        public async Task<bool> Save()
        {
            return Mode switch
            {
                SecurityHelper.EnMode.AddNewMode => await _AddNewPrescribedMedication(),
                SecurityHelper.EnMode.UpdateMode => await _UpdatePrescribedMedication(),
                SecurityHelper.EnMode.DeleteMode => await _DeletePrescribedMedication(),
                _ => false
            };
        }
    }
}
