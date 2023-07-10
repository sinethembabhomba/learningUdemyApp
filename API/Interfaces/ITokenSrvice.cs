using API.Entities;

namespace API.Interfaces
{
    public interface ITokenSrvice
    {
       Task<string> CreateToken(AppUser user); 
    }
}