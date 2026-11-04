using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;

namespace ClinicAppManagerBusinessLayer
{
    public class DataBusiness : IDataBusiness
    {
        private readonly IDataData _dataData;
        private SecurityHelper.EnMode _mode;

        public DataBusiness(IDataData dataData)
        {
            _dataData = dataData;
            Data = new DataDTO();
        }

        public DataDTO Data { get; set; }

        public void Initialize(DataDTO data, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            Data = data;
            _mode = mode;
        }

        private async Task<bool> AddNewData()
        {
            if (Data == null || string.IsNullOrWhiteSpace(Data.AppUserId) || string.IsNullOrWhiteSpace(Data.RefreshToken))
            {
                return false;
            }

            var entity = new Data
            {
                AppUserId = Data.AppUserId,
                RefreshToken = Data.RefreshToken,
                RefreshTokenExpiryTime = Data.RefreshTokenExpiryTime ?? DateTime.UtcNow,
                RevokedDate = Data.RevokedDate
            };
            entity.RefreshToken = SecurityHelper.HashPassword(entity.RefreshToken);
            Data.Id = await _dataData.AddNewData(entity);
            return Data.Id.HasValue;
        }

        private async Task<bool> UpdateData()
        {
            if (Data == null || !Data.Id.HasValue)
            {
                return false;
            }

            return await _dataData.UpdateData(Data);
        }

        private async Task<bool> DeleteData()
        {
            if (Data == null || !Data.Id.HasValue)
            {
                return false;
            }

            return await _dataData.DeleteData(Data.Id.Value);
        }

        public async Task<DataDTO> GetDataById(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            var data = await _dataData.GetDataById(id);

            if (data == null)
                return null;

            return MapToDataDto(await _dataData.GetDataById(id));
        }

        public async Task<DataDTO> GetDataByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return null;
            }

            return MapToDataDto(await _dataData.GetDataByAspNetUserId(aspNetUserId));
        }

        public async Task<List<DataDTO>> GetAllData()
        {
            var list = await _dataData.GetAllData();
            if (list == null || !list.Any())
            {
                return new List<DataDTO>();
            }

            return list.Select(MapToDataDto).Where(data => data != null).ToList();
        }

        public async Task<bool> CheckDataExistByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return false;
            }

            return await _dataData.CheckDataExistByAspNetUserId(aspNetUserId);
        }

        public async Task<bool> DeleteRefreshTokenAfterGeneratingNewOne(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return false;
            }

            return await _dataData.DeleteRefreshTokenAfterGeneratingNewOne(aspNetUserId);
        }

        public async Task<bool> Save()
        {
            return _mode switch
            {
                SecurityHelper.EnMode.AddNewMode => await AddNewData(),
                SecurityHelper.EnMode.UpdateMode => await UpdateData(),
                SecurityHelper.EnMode.DeleteMode => await DeleteData(),
                _ => false
            };
        }

        private static DataDTO MapToDataDto(Data entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new DataDTO
            {
                Id = entity.Id,
                AppUserId = entity.AppUserId,
                RefreshToken = entity.RefreshToken,
                RefreshTokenExpiryTime = entity.RefreshTokenExpiryTime,
                RevokedDate = entity.RevokedDate
            };
        }
    }
}
