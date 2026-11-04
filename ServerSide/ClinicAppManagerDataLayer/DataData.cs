using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppManagerDataLayer
{
    public class DataData : IDataData
    {
        private readonly AppDbContext _context;

        public DataData(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int?> AddNewData(Data data)
        {
            await _context.Data.AddAsync(data);
            await _context.SaveChangesAsync();

            return data.Id;
        }

        public async Task<bool> UpdateData(DataDTO data)
        {
            var existingData = await _context.Data.FindAsync(data.Id);
            if (existingData == null)
            {
                return false;
            }

            existingData.AppUserId = string.IsNullOrWhiteSpace(data.AppUserId)
                ? existingData.AppUserId
                : data.AppUserId;
            existingData.RefreshToken = string.IsNullOrWhiteSpace(data.RefreshToken)
                ? existingData.RefreshToken
                : data.RefreshToken;
            existingData.RefreshTokenExpiryTime = data.RefreshTokenExpiryTime ?? existingData.RefreshTokenExpiryTime;
            existingData.RevokedDate = data.RevokedDate ?? existingData.RevokedDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteData(int id)
        {
            var existingData = await _context.Data.FindAsync(id);
            if (existingData == null)
            {
                return false;
            }

            _context.Data.Remove(existingData);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRefreshTokenAfterGeneratingNewOne(string aspNetUserId)
        {
            var existingData = _context.Data.Where(data => data.AppUserId == aspNetUserId);
            if (existingData == null)
            {
                return false;
            }


            foreach (var item in existingData)
            {
                _context.Data.Remove(item);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Data> GetDataById(int id)
        {
            return await _context.Data.FirstOrDefaultAsync(data => data.Id == id);
        }

        public async Task<Data> GetDataByAspNetUserId(string aspNetUserId)
        {
            return await _context.Data.FirstOrDefaultAsync(data => data.AppUserId == aspNetUserId);
        }

        public async Task<List<Data>> GetAllData()
        {
            return await _context.Data.ToListAsync();
        }

        public async Task<bool> CheckDataExistByAspNetUserId(string aspNetUserId)
        {
            return await _context.Data.AnyAsync(data => data.AppUserId == aspNetUserId);
        }
    }
}
