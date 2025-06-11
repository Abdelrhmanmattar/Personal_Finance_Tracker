using Core.DTO;
using Core.entities;
using Core.Result;
using System.Security.Claims;

namespace Core.Interfaces
{
    public interface IMainUser_Repo
    {
        Task<Result<User_appDTO>> Regisiter_User(Regisiter_DTO _DTO);
        Task<Result<UserAuthResponse_DTO>> LogIn_User(LogIn_DTO _DTO);
        Task<Users_App> GetCurrentUser(ClaimsPrincipal claims);
        Task<string> GetUserByEmail(string email);
    }


}
