using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer
{
    public class MedicalRecordData : IMedicalRecordData
    {
        private readonly AppDbContext _context;

        public MedicalRecordData(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Add new medical record
        public async Task<int?> AddNewMedicalRecord(MedicalRecord record)
        {
            if (record == null)
                return null;

            await _context.MedicalRecords.AddAsync(record);
            await _context.SaveChangesAsync();

            return record.RecordId;
        }

        // Get a single medical record by ID (only its own info)
        public async Task<MedicalRecord> GetMedicalRecordById(int recordId)
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RecordId == recordId);
        }

        //  Get all medical records for a specific appointment (without related data)
        public async Task<List<MedicalRecord>> GetMedicalRecordsByAppointmentId(int appointmentId)
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .Where(r => r.AppoinmentId == appointmentId)
                .ToListAsync();
        }

        // Get all medical records (simple list)
        public async Task<List<MedicalRecord>> GetAllMedicalRecords()
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .ToListAsync();
        }

        // ✏ Update an existing medical record
        public async Task<bool> UpdateMedicalRecord(DTO.MedicalRecordDTO recordDto)
        {
            var existingRecord = await _context.MedicalRecords.FindAsync(recordDto.RecordId);

            if (existingRecord != null)
            {
                // Only update fields that have new non-null or non-empty values
                existingRecord.VisitDescription = string.IsNullOrEmpty(recordDto.VisitDescription)
                    ? existingRecord.VisitDescription
                    : recordDto.VisitDescription;

                existingRecord.Diagnosis = string.IsNullOrEmpty(recordDto.Diagnosis)
                    ? existingRecord.Diagnosis
                    : recordDto.Diagnosis;

                existingRecord.AdditionalNotes = string.IsNullOrEmpty(recordDto.AdditionalNotes)
                    ? existingRecord.AdditionalNotes
                    : recordDto.AdditionalNotes;

                // For AppointmentId (int?), update only if provided
                existingRecord.AppoinmentId = recordDto.AppoinmentId.HasValue
                    ? recordDto.AppoinmentId.Value
                    : existingRecord.AppoinmentId;

                await _context.SaveChangesAsync();
                return true; // Update successful
            }

            return false; // Record not found
        }
        
        //  Delete a medical record
        public async Task<bool> DeleteMedicalRecord(int recordId)
        {
            var record = await _context.MedicalRecords.FindAsync(recordId);
            if (record == null)
                return false;

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
