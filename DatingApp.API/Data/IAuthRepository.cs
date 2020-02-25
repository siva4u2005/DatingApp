using System.Threading.Tasks;
using DatingApp.API.Models;
namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user,string password);
         Task<User> LogIn(string userName,string password);
         Task<bool> UserExists(string userName);

    }

}