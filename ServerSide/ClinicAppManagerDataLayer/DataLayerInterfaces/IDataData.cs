using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IDataData
    {
        Task<int?> AddNewData(Data data);
        Task<bool> UpdateData(DataDTO data);
        Task<bool> DeleteData(int id);
        Task<bool> DeleteRefreshTokenAfterGeneratingNewOne(string aspNetUserId);
        Task<Data> GetDataById(int id);
        Task<Data> GetDataByAspNetUserId(string aspNetUserId);
        Task<List<Data>> GetAllData();
        Task<bool> CheckDataExistByAspNetUserId(string aspNetUserId);
    }
}
