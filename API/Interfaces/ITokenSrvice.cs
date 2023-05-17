using API.Entities;

namespace API.Interfaces
{
    public interface ITokenSrvice
    {
       string CreateToken(AppUser user); 
    }
}