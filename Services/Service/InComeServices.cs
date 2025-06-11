using Core.DTO;
using Core.entities;
using Core.Interfaces;
using Core.Result;
using Core.Specification;
using Repository.Specification;
using Services.caching;
using System.Security.Claims;

namespace Services.Service
{
    public class InComeServices : IIncome
    {
        private readonly IUnitOfWork unitWork;
        private readonly IcacheServices _cache;
        private readonly IMainUser_Repo mainUser_;

        public InComeServices(IUnitOfWork _unitWork, IcacheServices cache, IMainUser_Repo mainUser_)
        {
            unitWork = _unitWork;
            _cache = cache;
            this.mainUser_ = mainUser_;
        }
        public async Task<Result<IncomeDTO>> Create(ClaimsPrincipal claims, IncomeDTO dTO)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (dTO == null || user == null)
                return Result<IncomeDTO>.Fail(null, "unexpected error happen");
            string ID = user.Id;
            Income income = new Income()
            {
                Amount = dTO.Amount,
                Source = dTO.Source,
                Date_Deposite = DateTime.Now,
                User_Id = ID
            };
            var res = unitWork.Repository<Income>().AddEntity(income);
            unitWork.SaveChanges();
            dTO.Id = income.Id;
            await _cache.Remove($"Income-{ID}");
            return (res == true) ?
                Result<IncomeDTO>.Success(dTO)
                :
                Result<IncomeDTO>.Fail(dTO, "can't add");
        }


        public async Task<Result<IReadOnlyList<IncomeDTO>>> GetAll(ClaimsPrincipal claims)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IReadOnlyList<IncomeDTO>>.Fail(null, "error authorized");
            string ID = user.Id;

            IEnumerable<IncomeDTO> cachedIncomes = await _cache.GetAllfromCache<IncomeDTO>(ID);

            IEnumerable<IncomeDTO> res = [];
            if (cachedIncomes is null)
            {
                var spec = new BaseSpecification<Income>(s => s.User_Id == ID);
                res = unitWork.Repository<Income>().
                   FindAll(spec)
                   .Select(s => new IncomeDTO
                   {
                       Id = s.Id,
                       Source = s.Source,
                       Amount = s.Amount,
                       Date_Deposite = s.Date_Deposite
                   }
                   );
                await _cache.SetAsync<IEnumerable<IncomeDTO>>($"Income-{ID}", res);
            }
            else
            {
                res = cachedIncomes;
            }


            return (res != null) ?
                Result<IReadOnlyList<IncomeDTO>>.Success(res.ToList().AsReadOnly()) :
                Result<IReadOnlyList<IncomeDTO>>.Fail(null, "error in values");
        }
        public async Task<Result<IncomeDTO>> GetById(ClaimsPrincipal claims, int _idCome)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IncomeDTO>.Fail(null, "error authorized");

            string ID = user.Id;

            IEnumerable<IncomeDTO> cachedIncomes = await _cache.GetAllfromCache<IncomeDTO>(ID);

            if (cachedIncomes != null)
            {
                IncomeDTO entity = cachedIncomes.FirstOrDefault(i => i.Id == _idCome);
                return Result<IncomeDTO>.Success(entity);
            }

            var specs = new BaseSpecification<Income>
                (s => (s.Id == _idCome && s.User_Id == ID));

            var value = unitWork.Repository<Income>()
                .Find(specs);

            if (value == null)
                return Result<IncomeDTO>.Fail(null, "error in values");

            var res = new IncomeDTO()
            {
                Id = value.Id,
                Amount = value.Amount,
                Date_Deposite = value.Date_Deposite,
                Source = value.Source
            };
            return Result<IncomeDTO>.Success(res);
        }
        public async Task<Result<IncomeDTO>> Update(ClaimsPrincipal claims, int _idCome, IncomeDTO dTO)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null || dTO == null)
                return Result<IncomeDTO>.Fail(null, "unexpected error happen");

            string ID = user.Id;
            var spec = new BaseSpecification<Income>(i => i.Id == _idCome && i.User_Id == ID);

            var origin = unitWork.Repository<Income>().Find(spec);

            if (origin == null)
                return Result<IncomeDTO>.Fail(null, "Income not found");

            origin.Amount = dTO.Amount;
            origin.Source = dTO.Source;
            origin.Date_Deposite = dTO.Date_Deposite;

            var res = unitWork.Repository<Income>().UpdateEntity(origin);
            unitWork.SaveChanges();
            await _cache.Remove($"Income-{ID}");

            return res ?
                Result<IncomeDTO>.Success(dTO) :
                Result<IncomeDTO>.Fail(null, "can't update");
        }
        public async Task<Result<IncomeDTO>> Remove(ClaimsPrincipal claims, int idIncome)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IncomeDTO>.Fail(null, "unexpected error happen");

            string ID = user.Id;
            var specs = new BaseSpecification<Income>(i => i.Id == idIncome && i.User_Id == ID);

            var origin = unitWork.Repository<Income>().Find(specs);
            if (origin == null)
                return Result<IncomeDTO>.Fail(null, "Income not found");
            var res = unitWork.Repository<Income>().DeleteEntity(idIncome);
            unitWork.SaveChanges();

            await _cache.Remove($"Income-{ID}");

            return res ?
                Result<IncomeDTO>.Success(null) :
                Result<IncomeDTO>.Fail(null, "can't update");
        }
        public async Task<Result<IReadOnlyList<IncomeDTO>>> GetByDateRange(ClaimsPrincipal claims, DateTime From, DateTime To)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IReadOnlyList<IncomeDTO>>.Fail(null, "unexpected error happen");
            string ID = user.Id;

            IEnumerable<IncomeDTO> cachedIncomes = await _cache.GetAllfromCache<IncomeDTO>(ID);

            if (cachedIncomes != null)
            {
                var specsDTO = new BaseSpecification<IncomeDTO>
                    (i => (i.Date_Deposite >= From && i.Date_Deposite <= To));
                var queryValues = SpecificationHandler<IncomeDTO>
                    .ApplySpecification(cachedIncomes.AsQueryable(), specsDTO)
                    .ToList().AsReadOnly();

                return Result<IReadOnlyList<IncomeDTO>>.Success(queryValues);
            }

            var specs = new BaseSpecification<Income>
                (i => (i.User_Id == ID && i.Date_Deposite >= From && i.Date_Deposite <= To));

            var values = unitWork.Repository<Income>()
                            .FindAll(specs)
                            .Select(i => new IncomeDTO
                            {
                                Id = i.Id,
                                Amount = i.Amount,
                                Source = i.Source,
                                Date_Deposite = i.Date_Deposite
                            }).ToList().AsReadOnly();
            return Result<IReadOnlyList<IncomeDTO>>.Success(values);
        }
    }
}
