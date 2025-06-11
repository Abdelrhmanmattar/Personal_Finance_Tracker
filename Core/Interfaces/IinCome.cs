using System.Security.Claims;
using Core.DTO;
using Core.Result;

namespace Core.Interfaces
{
    public interface IIncome
    {
        Task<Result<IncomeDTO>> Create(ClaimsPrincipal claims, IncomeDTO dTO);
        Task<Result<IReadOnlyList<IncomeDTO>>> GetAll(ClaimsPrincipal claims);
        Task<Result<IncomeDTO>> GetById(ClaimsPrincipal claims, int _idCome);
        Task<Result<IncomeDTO>> Update(ClaimsPrincipal claims, int _idCome, IncomeDTO dTO);
        Task<Result<IncomeDTO>> Remove(ClaimsPrincipal claims, int idIncome);
        Task<Result<IReadOnlyList<IncomeDTO>>> GetByDateRange(ClaimsPrincipal claims, DateTime From, DateTime To);
    }
}
