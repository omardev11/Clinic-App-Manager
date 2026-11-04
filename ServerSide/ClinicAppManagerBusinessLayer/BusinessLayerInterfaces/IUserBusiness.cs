using ClinicAppManagerDataLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer.BusinessLayerInterfaces
{
    public interface IUserBusiness
    {
        public Task<UserDTO> GetUserInfoById(int UserId);
        public Task<bool> CheckUserNameExist(string UserName);
        public Task<bool> CheckUserExistByAspNetUserId(string aspNetUserId);
        public Task<UserDTO> GetUserInfoByFullName(string FullName);
        public Task<UserDTO> GetUserInfoByUserName(string UserName);
        public Task<UserDTO> GetUserInfoByAspNetUserId(string aspNetUserId);
        public Task<List<UserDTO>> GetAllUsers();
        public void Initialize(UserDTO User, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode);
        public UserDTO User { get; set; }

        public Task<bool> Save();
    }
}
