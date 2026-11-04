using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DataLayerInterfaces
{
    public interface IUserData
    {
        public Task<int?> AddNewUser(Entities.User User);
        public Task<Entities.User> GetUserInfoById(int UserId);
        public Task<Entities.User> GetUserInfoByFullName(string FullName);
        public Task<Entities.User> GetUserInfoByUserName(string UserName);
        public Task<Entities.User> GetUserInfoByAspNetUserId(string aspNetUserId);
        public Task<List<Entities.User>> GetAllUsers();
        public Task<bool> UpdateUserInfo(DTO.UserDTO User);
        public Task<bool> CheckUserNameExist(string UserName);
        public Task<bool> CheckUserExistByAspNetUserId(string aspNetUserId);
    }
}
