using Core.DTO;
using Core.entities;
using Core.Result;
using System.Security.Claims;

namespace Core.Interfaces
{
    public interface ICategory
    {
        Task<Result<CategoryDTO>> Add(ClaimsPrincipal claims, CategoryDTO dTO);
        Task<Result<CategoryDTO>> Remove(ClaimsPrincipal claims, int _idCat);
        Task<Result<IReadOnlyList<CategoryDTO>>> GetAll(ClaimsPrincipal claims);
    }


}
