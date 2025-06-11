using Core.DTO;
using Core.entities;
using Core.Interfaces;
using Core.Result;
using Core.Specification;
using Services.caching;
using System.Security.Claims;

namespace Services.Service
{
    public class BudgetServices : IBudget
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IcacheServices _cache;
        private readonly IMainUser_Repo mainUser_;

        public BudgetServices(IUnitOfWork _unitOfWork, IcacheServices cache, IMainUser_Repo mainUser_)
        {
            unitOfWork = _unitOfWork;
            _cache = cache;
            this.mainUser_ = mainUser_;
        }
        public async Task<Result<BudgetDTO>> Create(ClaimsPrincipal claims, BudgetDTO dTO)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (dTO == null || user == null)
                return Result<BudgetDTO>.Fail(null, "unexpected error happen");
            string ID = user.Id;
            Budgets budget = new Budgets()
            {
                User_Id = ID,
                Cat_Id = dTO.Cat_Id,
                LimitAmount = dTO.LimitAmount,
            };
            var res = unitOfWork.Repository<Budgets>().AddEntity(budget);
            unitOfWork.SaveChanges();
            dTO.ID = budget.Id;

            await _cache.Remove($"Budget-{ID}");
            return (res == true) ?
                Result<BudgetDTO>.Success(dTO)
                :
                Result<BudgetDTO>.Fail(dTO, "can't add");
        }
        public async Task<Result<IReadOnlyList<BudgetDTO>>> GetAll(ClaimsPrincipal claims)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IReadOnlyList<BudgetDTO>>.Fail(null, "error authorized");
            string ID = user.Id;
            var specs = new BaseSpecification<Budgets>(b => b.User_Id == ID);

            IEnumerable<BudgetDTO> cachedBudget = await _cache.GetAllfromCache<BudgetDTO>(ID);

            IEnumerable<BudgetDTO> res = [];
            if (cachedBudget is null)
            {
                res = unitOfWork.Repository<Budgets>()
                    .FindAll(specs)
                    .Select(b => new BudgetDTO
                    {
                        ID = b.Id,
                        Cat_Id = b.Cat_Id,
                        LimitAmount = b.LimitAmount,
                    }
                    );
                await _cache.SetAsync<IEnumerable<BudgetDTO>>($"Budget-{ID}", res);
            }
            else
            {
                res = cachedBudget;
            }

            return (res != null) ?
                Result<IReadOnlyList<BudgetDTO>>.Success(res.ToList().AsReadOnly()) :
                Result<IReadOnlyList<BudgetDTO>>.Fail(null, "error in values");
        }
        public async Task<Result<BudgetDTO>> GetById(ClaimsPrincipal claims, int _idBudget)
        {
            var user = await mainUser_.GetCurrentUser(claims);

            if (user == null)
                return Result<BudgetDTO>.Fail(null, "error authorized");

            string ID = user.Id;
            //try to get from cached data
            IEnumerable<BudgetDTO> cachedBudgets = await _cache.GetAllfromCache<BudgetDTO>(ID);
            if (cachedBudgets != null)
            {
                var cachedEntity = cachedBudgets.FirstOrDefault(d => d.ID == _idBudget);
                return Result<BudgetDTO>.Success(cachedEntity);
            }

            //not found in cached

            var specs = new BaseSpecification<Budgets>
                (s => (s.Id == _idBudget && s.User_Id == ID));

            var value = unitOfWork.Repository<Budgets>()
                .Find(specs);

            if (value == null) return Result<BudgetDTO>.Fail(null, "error in values");

            var res = new BudgetDTO()
            {
                ID = value.Id,
                Cat_Id = value.Cat_Id,
                LimitAmount = value.LimitAmount,
            };
            return Result<BudgetDTO>.Success(res);
        }
        public async Task<Result<BudgetDTO>> Update(ClaimsPrincipal claims, BudgetDTO budget)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null || budget == null)
                return Result<BudgetDTO>.Fail(null, "unexpected error happen");

            string ID = user.Id;
            var specs = new BaseSpecification<Budgets>(b => b.Id == budget.ID && b.User_Id == ID);
            var origin = unitOfWork.Repository<Budgets>().Find(specs);
            if (origin == null)
                return Result<BudgetDTO>.Fail(null, "Income not found");

            origin.Cat_Id = budget.Cat_Id;
            origin.LimitAmount = budget.LimitAmount;

            var res = unitOfWork.Repository<Budgets>().UpdateEntity(origin);
            unitOfWork.SaveChanges();

            await _cache.Remove($"Budget-{ID}");

            return res ?
                Result<BudgetDTO>.Success(budget) :
                Result<BudgetDTO>.Fail(null, "can't update");
        }
        public async Task<Result<BudgetDTO>> Remove(ClaimsPrincipal claims, int idBudget)
        {
            var user = await mainUser_.GetCurrentUser(claims);

            if (user == null)
                return Result<BudgetDTO>.Fail(null, "unexpected error happen");

            string ID = user.Id;
            var specs = new BaseSpecification<Budgets>(b => b.Id == idBudget && b.User_Id == ID);
            var origin = unitOfWork.Repository<Budgets>().Find(specs);
            if (origin == null)
                return Result<BudgetDTO>.Fail(null, "Category not found");

            var res = unitOfWork.Repository<Budgets>().DeleteEntity(idBudget);
            unitOfWork.SaveChanges();
            await _cache.Remove($"Budget-{ID}");

            return res ?
                Result<BudgetDTO>.Success(null) :
                Result<BudgetDTO>.Fail(null, "can't Delete");
        }
    }


}
