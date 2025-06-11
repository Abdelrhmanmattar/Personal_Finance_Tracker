using Core.DTO;
using Core.entities;
using Core.Interfaces;
using Core.Result;
using Core.Specification;
using Services.caching;
using System.Security.Claims;

namespace Services.Service
{
    public class ClategoryServices : ICategory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IcacheServices _cache;
        private readonly IMainUser_Repo mainUser_;

        public ClategoryServices(IUnitOfWork _unitOfWork, IcacheServices cache, IMainUser_Repo mainUser_)
        {
            unitOfWork = _unitOfWork;
            _cache = cache;
            this.mainUser_ = mainUser_;
        }
        public async Task<Result<CategoryDTO>> Add(ClaimsPrincipal claims, CategoryDTO dTO)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (dTO == null || user == null)
                return Result<CategoryDTO>.Fail(null, "unexpected error happen");
            string ID = user.Id;
            Category category = new Category()
            {
                Name = dTO.Name,
                User_Id = ID,
            };
            var res = unitOfWork.Repository<Category>().AddEntity(category);
            unitOfWork.SaveChanges();
            await _cache.Remove($"Category-{ID}");
            return (res == true) ?
                Result<CategoryDTO>.Success(dTO)
                :
                Result<CategoryDTO>.Fail(dTO, "can't add");
        }
        public async Task<Result<CategoryDTO>> Remove(ClaimsPrincipal claims, int _idCat)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<CategoryDTO>.Fail(null, "unexpected error happen");
            string ID = user.Id;

            var specs = new BaseSpecification<Category>(c => c.User_Id == ID && c.Id == _idCat);

            var origin = unitOfWork.Repository<Category>().Find(specs);
            if (origin == null)
                return Result<CategoryDTO>.Fail(null, "Category not found");

            var res = unitOfWork.Repository<Category>().DeleteEntity(_idCat);
            unitOfWork.SaveChanges();
            await _cache.Remove($"Category-{ID}");

            return res ?
                Result<CategoryDTO>.Success(null) :
                Result<CategoryDTO>.Fail(null, "can't update");
        }
        public async Task<Result<IReadOnlyList<CategoryDTO>>> GetAll(ClaimsPrincipal claims)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IReadOnlyList<CategoryDTO>>.Fail(null, "unexpected error happen");
            string ID = user.Id;

            IEnumerable<CategoryDTO> cachedCategory = await _cache.GetAllfromCache<CategoryDTO>(ID);

            IEnumerable<CategoryDTO> res = [];
            if (cachedCategory is null)
            {
                var specs = new BaseSpecification<Category>(c => c.User_Id == ID);
                res = unitOfWork.Repository<Category>()
                      .FindAll(specs).Select(c => new CategoryDTO
                      {
                          Id = c.Id,
                          Name = c.Name,
                      }).ToList();
                await _cache.SetAsync<IEnumerable<CategoryDTO>>($"Category-{ID}", res);
            }
            else
            {
                res = cachedCategory;
            }

            return (res != null) ?
                Result<IReadOnlyList<CategoryDTO>>.Success(res.ToList().AsReadOnly()) :
                Result<IReadOnlyList<CategoryDTO>>.Fail(null, "error in values");
        }

    }
}
