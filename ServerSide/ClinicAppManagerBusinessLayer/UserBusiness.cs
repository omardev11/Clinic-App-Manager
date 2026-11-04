using ClinicAppManagerBusinessLayer.BusinessLayerInterfaces;
using ClinicAppManagerDataLayer.DataLayerInterfaces;
using ClinicAppManagerDataLayer.DTO;
using ClinicAppManagerDataLayer.Entities;

namespace ClinicAppManagerBusinessLayer
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserData _UserData;
        public UserDTO User { get; set; }
        private SecurityHelper.EnMode Mode;

        public UserBusiness(IUserData UserData)
        {
            _UserData = UserData;
            User = new UserDTO();
        }

        public void Initialize(UserDTO User, SecurityHelper.EnMode mode = SecurityHelper.EnMode.AddNewMode)
        {
            this.User = User;
            Mode = mode;
        }

        private async Task<bool> _AddNewUser()
        {
            if (User == null || string.IsNullOrWhiteSpace(User.AspNetUserId))
            {
                return false;
            }

            var userEntity = new User
            {
                AspNetUserId = User.AspNetUserId,
                UserName = User.UserName,
                IsActive = User.IsActive ?? true,
                Person = new Person
                {
                    FirstName = User.Person.FirstName,
                    LastName = User.Person.LastName,
                    DateOfBirth = User.Person.DateOfBirth!.Value,
                    Address = User.Person.Address,
                    Gender = User.Person.Gender!.Value
                }
            };

            User.UserId = await _UserData.AddNewUser(userEntity);
            return User.UserId.HasValue;
        }

        private async Task<bool> _UpdateUser()
        {
            if (User == null || !User.UserId.HasValue)
            {
                return false;
            }

            return await _UserData.UpdateUserInfo(User);
        }

        public async Task<UserDTO> GetUserInfoById(int UserId)
        {
            if (UserId <= 0)
            {
                return null;
            }

            var entityUser = await _UserData.GetUserInfoById(UserId);
            return MapToUserDto(entityUser);
        }

        public async Task<UserDTO> GetUserInfoByFullName(string FullName)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return null;
            }

            var entityUser = await _UserData.GetUserInfoByFullName(FullName);
            return MapToUserDto(entityUser);
        }

        public async Task<UserDTO> GetUserInfoByUserName(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                return null;
            }

            var entityUser = await _UserData.GetUserInfoByUserName(UserName);
            return MapToUserDto(entityUser);
        }

        public async Task<UserDTO> GetUserInfoByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return null;
            }

            var entityUser = await _UserData.GetUserInfoByAspNetUserId(aspNetUserId);
            return MapToUserDto(entityUser);
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var entityUsers = await _UserData.GetAllUsers();
            if (entityUsers == null || !entityUsers.Any())
            {
                return new List<UserDTO>();
            }

            return entityUsers
                .Select(MapToUserDto)
                .Where(user => user != null)
                .ToList();
        }

        public async Task<bool> CheckUserNameExist(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                return false;
            }

            return await _UserData.CheckUserNameExist(UserName);
        }

        public async Task<bool> CheckUserExistByAspNetUserId(string aspNetUserId)
        {
            if (string.IsNullOrWhiteSpace(aspNetUserId))
            {
                return false;
            }

            return await _UserData.CheckUserExistByAspNetUserId(aspNetUserId);
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case SecurityHelper.EnMode.AddNewMode:
                    return await _AddNewUser();
                case SecurityHelper.EnMode.UpdateMode:
                    return await _UpdateUser();
                default:
                    return false;
            }
        }

        private static UserDTO MapToUserDto(User entityUser)
        {
            if (entityUser == null)
            {
                return null;
            }

            return new UserDTO
            {
                UserId = entityUser.UserId,
                AspNetUserId = entityUser.AspNetUserId,
                UserName = entityUser.UserName,
                PersonId = entityUser.PersonId,
                IsActive = entityUser.IsActive,
                Person = new PersonDTO
                {
                    FirstName = entityUser.Person.FirstName,
                    LastName = entityUser.Person.LastName,
                    DateOfBirth = entityUser.Person.DateOfBirth,
                    Address = entityUser.Person.Address,
                    Age = entityUser.Person.Age,
                    FullName = entityUser.Person.FullName,
                    Gender = entityUser.Person.Gender
                }
            };
        }
    }
}
