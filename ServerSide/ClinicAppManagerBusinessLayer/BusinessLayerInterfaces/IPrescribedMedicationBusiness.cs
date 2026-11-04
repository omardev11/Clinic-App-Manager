using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IPrescribedMedicationBusiness
    {
        PrescribedMedicationDTO PrescribedMedication { get; set; }
        void Initialize(PrescribedMedicationDTO PrescribedMedication, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        Task<bool> Save();
        Task<PrescribedMedicationDTO?> GetPrescribedMedicationById(int PrescribedMedicationId);
        Task<PrescribedMedicationDTO?> GetPrescribedMedicationByRecordId(int RecordId);

        Task<List<PrescribedMedicationDTO>> GetAllPrescribedMedications();
    }
}
