using System.Security.Claims;
using Core.DTO;
using Core.Result;

namespace Core.Interfaces
{
    public interface IBudgetUser
    {
        Task<Result<BudgetUserDTO>> AddUserToBudget(ClaimsPrincipal claims, string email, int idBudget);
        Task<Result<BudgetUserDTO>> RemoveUserFromBudget(ClaimsPrincipal claims, string email, int idBudget);
        Task<Result<IReadOnlyList<User_appDTO>>> UsersSubscribeInAdminBudget(ClaimsPrincipal claims, int idBudget);
        Task<Result<IReadOnlyList<WatcherTreeDTO>>> MyBudgetsAsWatcher(ClaimsPrincipal claims);
        Task<Result<IEnumerable<string>>> IDsSubscribeInAdminBudget(ClaimsPrincipal claims, int idBudget);

    }
}
