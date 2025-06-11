using System.Security.Claims;
using Core.DTO;
using Core.entities;
using Core.Result;

namespace Core.Interfaces
{
    public interface IExpense
    {
        Task<Result<ExpensesDTO>> AddExpense(ExpensesDTO dTO, ClaimsPrincipal claims);
        Task<Result<IReadOnlyList<ExpensesDTO>>> GetAll(ClaimsPrincipal claims);
        Task<Result<ExpensesDTO>> GetById(ClaimsPrincipal claims, int _idExpenses);
        Task<Result<ExpensesDTO>> Update(ClaimsPrincipal claims, ExpensesDTO dto);
        Task<Result<ExpensesDTO>> Remove(ClaimsPrincipal claims, int _idExpenses);
        Task<Result<object>> GetTotalExpenses(ClaimsPrincipal claims,int Cat_Id);
        Task<Result<IReadOnlyList<ExpensesDTO>>> GetByDateRange(ClaimsPrincipal claims, int CatID, DateTime From, DateTime To);
    }
}
