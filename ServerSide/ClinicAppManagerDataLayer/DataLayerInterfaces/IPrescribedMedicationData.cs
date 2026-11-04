using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IPrescribedMedicationData
    {
        public Task<int> AddPrescribedMedicationAsync(PrescribedMedication PrescribedMedication);
        public Task<bool> UpdatePrescribedMedicationAsync(PrescribedMedicationDTO PrescribedMedication);
        public Task<bool> DeletePrescribedMedicationAsync(int prescriptionId);
        public Task<PrescribedMedication> GetPrescribedMedicationByIdAsync(int PrescribedMedicationId);
        public Task<PrescribedMedication> GetPrescribedMedicationByRecordIdAsync(int AppointemtId);

        public Task<List<PrescribedMedication>> GetAllPrescribedMedicationsAsync();
    }
}
