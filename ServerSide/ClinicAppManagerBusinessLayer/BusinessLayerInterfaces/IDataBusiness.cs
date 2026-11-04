using ClinicAppManagerDataLayer.DTO;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IDataBusiness
    {
        DataDTO Data { get; set; }
        void Initialize(DataDTO data, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        Task<bool> Save();
        Task<DataDTO> GetDataById(int id);
        Task<DataDTO> GetDataByAspNetUserId(string aspNetUserId);
        Task<List<DataDTO>> GetAllData();
        Task<bool> CheckDataExistByAspNetUserId(string aspNetUserId);
        Task<bool> DeleteRefreshTokenAfterGeneratingNewOne(string aspNetUserId);
    }
}
