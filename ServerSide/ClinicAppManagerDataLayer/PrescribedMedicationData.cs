using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer
{
    public class PrescribedMedicationData : IPrescribedMedicationData
    {
        private readonly AppDbContext _context;

        public PrescribedMedicationData(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddPrescribedMedicationAsync(PrescribedMedication prescribedMedication)
        {
            await _context.PrescribedMedications.AddAsync(prescribedMedication);
            await _context.SaveChangesAsync();
            return prescribedMedication.PrescriptionId;
        }

        public async Task<bool> UpdatePrescribedMedicationAsync(PrescribedMedicationDTO medication)
        {
            var existingMedication = await _context.PrescribedMedications.FindAsync(medication.PrescriptionId);
            if (existingMedication == null)
                return false;

            existingMedication.MedicatoinName = medication.MedicatoinName ?? existingMedication.MedicatoinName;
            existingMedication.Dosage = medication.Dosage ?? existingMedication.Dosage;
            existingMedication.Frequency = medication.Frequency ?? existingMedication.Frequency;
            existingMedication.StartDate = medication.StartDate.HasValue ? medication.StartDate.Value : existingMedication.StartDate;
            existingMedication.EndDate = medication.EndDate.HasValue ? medication.EndDate.Value : existingMedication.EndDate;
            existingMedication.SpecialInstructions = medication.SpecialInstructions ?? existingMedication.SpecialInstructions;
            existingMedication.RecordId = medication.RecordId.HasValue ? medication.RecordId.Value : existingMedication.RecordId;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePrescribedMedicationAsync(int prescriptionId)
        {
            var existingMedication = await _context.PrescribedMedications.FindAsync(prescriptionId);
            if (existingMedication == null)
                return false;

            _context.PrescribedMedications.Remove(existingMedication);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PrescribedMedication?> GetPrescribedMedicationByIdAsync(int prescriptionId)
        {
            return await _context.PrescribedMedications
                .Where(p => p.PrescriptionId == prescriptionId)
                .Select(p => new PrescribedMedication
                {
                    PrescriptionId = p.PrescriptionId,
                    MedicatoinName = p.MedicatoinName,
                    Dosage = p.Dosage,
                    Frequency = p.Frequency,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    SpecialInstructions = p.SpecialInstructions,
                    RecordId = p.RecordId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PrescribedMedication?> GetPrescribedMedicationByRecordIdAsync(int recordId)
        {
            return await _context.PrescribedMedications
                .Where(p => p.RecordId == recordId)
                .Select(p => new PrescribedMedication
                {
                    PrescriptionId = p.PrescriptionId,
                    MedicatoinName = p.MedicatoinName,
                    Dosage = p.Dosage,
                    Frequency = p.Frequency,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    SpecialInstructions = p.SpecialInstructions,
                    RecordId = p.RecordId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<PrescribedMedication>> GetAllPrescribedMedicationsAsync()
        {
            return await _context.PrescribedMedications
                .Select(p => new PrescribedMedication
                {
                    PrescriptionId = p.PrescriptionId,
                    MedicatoinName = p.MedicatoinName,
                    Dosage = p.Dosage,
                    Frequency = p.Frequency,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    SpecialInstructions = p.SpecialInstructions,
                    RecordId = p.RecordId
                })
                .ToListAsync();
        }
    }
}
