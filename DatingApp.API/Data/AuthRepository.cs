using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> LogIn(string userName, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName==userName);

            if(user == null)
            return null;

            if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
            return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            for(int i=0;i<computedHash.Length;i++)
            {
                if(computedHash[i]!=passwordHash[i]) return false;
            }

            }

            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash,passwordsalt;
            CreatePasswordHash(password,out passwordHash, out passwordsalt);
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordsalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordsalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
            passwordsalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string userName)
        {

            if(await _context.Users.AnyAsync(x=>x.UserName==userName)) 
            return true;

            return false;
            
        }
    }
}