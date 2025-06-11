using Core.DTO;
using Core.Result;
using System.Security.Claims;

namespace Core.Interfaces
{
    public interface IBudget
    {
        Task<Result<BudgetDTO>> Create(ClaimsPrincipal claims, BudgetDTO dTO);
        Task<Result<IReadOnlyList<BudgetDTO>>> GetAll(ClaimsPrincipal claims);
        Task<Result<BudgetDTO>> GetById(ClaimsPrincipal claims, int _idBudget);
        Task<Result<BudgetDTO>> Update(ClaimsPrincipal claims, BudgetDTO budget);
        Task<Result<BudgetDTO>> Remove(ClaimsPrincipal claims, int idBudget);
    }


}
