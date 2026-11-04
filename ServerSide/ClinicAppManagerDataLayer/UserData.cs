using ClinicAppManagerDataLayer.DataLayerInterfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppManagerDataLayer
{
    public class UserData : IUserData
    {
        private readonly AppDbContext _Context;
        private readonly IPersonData _PersonData;

        public UserData(AppDbContext context, IPersonData personData)
        {
            _Context = context ?? throw new ArgumentNullException(nameof(context));
            _PersonData = personData ?? throw new ArgumentNullException(nameof(personData));
        }

        public async Task<int?> AddNewUser(Entities.User User)
        {
            int? personId = await _PersonData.AddNewPerson(User.Person);

            if (personId == null)
            {
                return null;
            }

            User.PersonId = personId.Value;
            await _Context.Users.AddAsync(User);
            await _Context.SaveChangesAsync();

            return User.UserId;
        }

        public async Task<Entities.User> GetUserInfoById(int UserId)
        {
            var user = await _Context.Users
                .FirstOrDefaultAsync(p => p.UserId == UserId);

            if (user != null)
            {
                user.Person = await _PersonData.GetPersonInfoById(user.PersonId);
            }

            return user;
        }

        public async Task<Entities.User> GetUserInfoByFullName(string FullName)
        {
            var user = await _Context.Users
                .Include(p => p.Person)
                .FirstOrDefaultAsync(p => p.Person.FullName == FullName);

            if (user != null)
            {
                user.Person = await _PersonData.GetPersonInfoById(user.PersonId);
            }

            return user;
        }

        public async Task<Entities.User> GetUserInfoByUserName(string UserName)
        {
            var user = await _Context.Users
                .FirstOrDefaultAsync(p => p.UserName == UserName);

            if (user != null)
            {
                user.Person = await _PersonData.GetPersonInfoById(user.PersonId);
            }

            return user;
        }

        public async Task<Entities.User> GetUserInfoByAspNetUserId(string aspNetUserId)
        {
            var user = await _Context.Users
                .FirstOrDefaultAsync(p => p.AspNetUserId == aspNetUserId);

            if (user != null)
            {
                user.Person = await _PersonData.GetPersonInfoById(user.PersonId);
            }

            return user;
        }

        public async Task<List<Entities.User>> GetAllUsers()
        {
            var users = await _Context.Users.ToListAsync();

            foreach (var user in users)
            {
                user.Person = await _PersonData.GetPersonInfoById(user.PersonId);
            }

            return users;
        }

        public async Task<bool> UpdateUserInfo(DTO.UserDTO User)
        {
            var existingUser = await _Context.Users.FindAsync(User.UserId);

            if (existingUser == null)
            {
                return false;
            }

            User.Person.PersonId = existingUser.PersonId;

            if (!await _PersonData.UpdatePersonInfo(User.Person))
            {
                return false;
            }

            existingUser.UserName = string.IsNullOrEmpty(User.UserName)
                ? existingUser.UserName
                : User.UserName;
            existingUser.IsActive = User.IsActive ?? existingUser.IsActive;

            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckUserNameExist(string UserName)
        {
            return await _Context.Users.AnyAsync(p => p.UserName == UserName);
        }

        public async Task<bool> CheckUserExistByAspNetUserId(string aspNetUserId)
        {
            return await _Context.Users.AnyAsync(p => p.AspNetUserId == aspNetUserId);
        }
    }
}
