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
    public class ExpenseServices : IExpense
    {
        private readonly IUnitOfWork unitWork;
        private readonly IMainUser_Repo mainUser_;
        private readonly IBudgetUser budgetUser;
        private readonly ISignalRService signalR;
        private readonly IcacheServices _cache;
        private readonly INotificationServices _notificationServices;

        public ExpenseServices(IUnitOfWork _unitWork,
            IMainUser_Repo mainUser_,
            IBudgetUser budgetUser,
            ISignalRService _signalR, IcacheServices cache,
            INotificationServices notificationServices)
        {
            unitWork = _unitWork;
            this.mainUser_ = mainUser_;
            this.budgetUser = budgetUser;
            signalR = _signalR;
            _cache = cache;
            _notificationServices = notificationServices;
        }
        //need to check after update if break amountlimit?
        private async Task<bool> checkLimit(ClaimsPrincipal claims, int Cat_Id)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return false;
            string ID = user.Id;
            var specs = new BaseSpecification<Budgets>(
                b => (b.User_Id == ID && b.Id == Cat_Id)
                );

            var budgetLimitAmount = unitWork.Repository<Budgets>()
                            .Find(specs)?.LimitAmount ?? -1;

            if (budgetLimitAmount == -1)
                return false;

            var IsCategory = await this.GetTotalExpenses(claims, Cat_Id);

            if (IsCategory.IsSuccess == false)
                return false;

            var newtotal = ((dynamic)IsCategory.Data).TotalExpenses;
            return newtotal > budgetLimitAmount;
        }
        public async Task<Result<ExpensesDTO>> AddExpense(ExpensesDTO dTO, ClaimsPrincipal claims)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (dTO == null || user == null)
                return Result<ExpensesDTO>.Fail(null, "unexpected error happen");
            string ID = user.Id;

            var expense = new Expenses()
            {
                Amount = dTO.Amount,
                BudgetId = dTO.BudgetId,
                Date_Withdraw = DateTime.Now,
                User_Id = ID
            };
            var res = unitWork.Repository<Expenses>().AddEntity(expense);
            unitWork.SaveChanges();

            await _cache.Remove($"Expense-{ID}");

            if (await checkLimit(claims, dTO.BudgetId))
            {
                //we will get all budget member 
                var usersList = await budgetUser.IDsSubscribeInAdminBudget(claims, dTO.BudgetId);
                if (usersList.IsSuccess)
                {
                    var IDlist = usersList.Data.Append(ID);
                    var message = $"{dTO.Id} Budget is overflow after add {dTO.Amount}.by{user.Email}";
                    await notiSender(IDlist, message);
                }
            }
            return (res == true) ?
                Result<ExpensesDTO>.Success(dTO)
                :
                Result<ExpensesDTO>.Fail(dTO, "can't add");
        }
        //finish
        public async Task<Result<IReadOnlyList<ExpensesDTO>>> GetAll(ClaimsPrincipal claims)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IReadOnlyList<ExpensesDTO>>.Fail(null, "error authorized");
            string ID = user.Id;

            IEnumerable<ExpensesDTO> cachedExpenses = await _cache.GetAllfromCache<ExpensesDTO>(ID);

            IEnumerable<ExpensesDTO> res = [];
            if (cachedExpenses is null)
            {
                var specs = new BaseSpecification<Expenses>(e => e.User_Id == ID);

                res = unitWork.Repository<Expenses>()
                 .FindAll(specs)
                 .Select(e => new ExpensesDTO
                 {
                     Id = e.Id,
                     Amount = e.Amount,
                     BudgetId = e.BudgetId,
                     Date_Withdraw = e.Date_Withdraw,
                 }
                 );
                await _cache.SetAsync<IEnumerable<ExpensesDTO>>($"Expense-{ID}", res);
            }
            else
            {
                res = cachedExpenses;
            }
            return (res != null) ?
                Result<IReadOnlyList<ExpensesDTO>>.Success(res.ToList().AsReadOnly()) :
                Result<IReadOnlyList<ExpensesDTO>>.Fail(null, "error in values");
        }

        //finished
        public async Task<Result<ExpensesDTO>> GetById(ClaimsPrincipal claims, int _idExpenses)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<ExpensesDTO>.Fail(null, "error authorized");

            string ID = user.Id;

            IEnumerable<ExpensesDTO> cachedExpenses = await _cache.GetAllfromCache<ExpensesDTO>(ID);
            if (cachedExpenses != null)
            {
                var entity = cachedExpenses.FirstOrDefault(d => d.Id == _idExpenses);
                return Result<ExpensesDTO>.Success(entity);
            }


            var specs = new BaseSpecification<Expenses>
                (e => e.Id == _idExpenses && e.User_Id == ID);

            var value = unitWork.Repository<Expenses>().Find(specs);

            if (value == null)
                return Result<ExpensesDTO>.Fail(null, "error in values");

            var res = new ExpensesDTO()
            {
                Id = value.Id,
                Amount = value.Amount,
                BudgetId = value.BudgetId,
                Date_Withdraw = value.Date_Withdraw,
            };
            return Result<ExpensesDTO>.Success(res);
        }
        //need to check after update if break amountlimit?
        public async Task<Result<ExpensesDTO>> Update(ClaimsPrincipal claims, ExpensesDTO dto)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null || dto == null)
                return Result<ExpensesDTO>.Fail(null, "unexpected error happen");
            string ID = user.Id;
            var specs = new BaseSpecification<Expenses>(e => e.Id == dto.Id && e.User_Id == ID);
            var origin = unitWork.Repository<Expenses>().Find(specs);
            if (origin == null)
                return Result<ExpensesDTO>.Fail(null, "Income not found");
            Expenses expenses = new Expenses()
            {
                Id = dto.Id,
                Amount = dto.Amount,
                BudgetId = dto.BudgetId,
                Date_Withdraw = dto.Date_Withdraw
            };

            var res = unitWork.Repository<Expenses>().UpdateEntity(expenses);
            unitWork.SaveChanges();
            await _cache.Remove($"Expense-{ID}");


            if (await checkLimit(claims, dto.BudgetId))
            {
                //we will get all budget member 
                var usersList = await budgetUser.IDsSubscribeInAdminBudget(claims, dto.BudgetId);
                if (usersList.IsSuccess)
                {
                    var IDlist = usersList.Data.Append(ID);
                    var message = $"{dto.Id} Budget is overflow after update {dto.Amount}.by{user.Email}";
                    await notiSender(IDlist, message);
                }
            }



            return res ?
                Result<ExpensesDTO>.Success(dto) :
                Result<ExpensesDTO>.Fail(dto, "can't update");
        }
        //need to check after update if break amountlimit?
        public async Task<Result<ExpensesDTO>> Remove(ClaimsPrincipal claims, int _idExpenses)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<ExpensesDTO>.Fail(null, "unexpected error happen");
            string ID = user.Id;
            var specs = new BaseSpecification<Expenses>(e => e.Id == _idExpenses && e.User_Id == ID);

            var origin = unitWork.Repository<Expenses>().Find(specs);
            if (origin == null)
                return Result<ExpensesDTO>.Fail(null, "Income not found");

            var res = unitWork.Repository<Expenses>().DeleteEntity(_idExpenses);
            unitWork.SaveChanges();
            await _cache.Remove($"Expense-{ID}");

            return res ?
                Result<ExpensesDTO>.Success(null) :
                Result<ExpensesDTO>.Fail(null, "can't Remove");
        }

        public async Task<Result<object>> GetTotalExpenses(ClaimsPrincipal claims, int BudgetId)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<object>.Fail(null, "unexpected error happen");
            string ID = user.Id;
            var specs = new BaseSpecification<Expenses>(
                e => e.BudgetId == BudgetId && e.User_Id == ID);

            var values = unitWork.Repository<Expenses>()
                .FindAll(specs)
                .Select(s => s.Amount)
                .ToList();


            decimal? total = (values.Any()) ?
                             (values.Sum()) : 0;
            var specsbudget = new BaseSpecification<Budgets>(
                b => b.Id == BudgetId && b.User_Id == ID);
            specsbudget.AddInclude(b => b.Category);
            var catName = unitWork.Repository<Budgets>().Find(specsbudget)?.Category?.Name;

            return Result<object>.Success(new { BudgetId = BudgetId, Name = catName, TotalExpenses = total });
        }

        public async Task<Result<IReadOnlyList<ExpensesDTO>>> GetByDateRange(ClaimsPrincipal claims, int BudgetId, DateTime From, DateTime To)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<IReadOnlyList<ExpensesDTO>>.Fail(null, "unexpected error happen");
            string ID = user.Id;

            IEnumerable<ExpensesDTO> cachedExpenses = await _cache.GetAllfromCache<ExpensesDTO>(ID);
            if (cachedExpenses != null)
            {
                var specsDTO = new BaseSpecification<ExpensesDTO>
                    (i => (i.BudgetId == BudgetId && i.Date_Withdraw >= From && i.Date_Withdraw <= To));

                var queryValues = SpecificationHandler<ExpensesDTO>
                    .ApplySpecification(cachedExpenses.AsQueryable(), specsDTO)
                    .ToList().AsReadOnly();

                return Result<IReadOnlyList<ExpensesDTO>>.Success(queryValues);
            }
            var specs = new BaseSpecification<Expenses>
                (i => (i.User_Id == ID && i.BudgetId == BudgetId && i.Date_Withdraw >= From && i.Date_Withdraw <= To));

            var values = unitWork.Repository<Expenses>()
                            .FindAll(specs)
                            .Select(i => new ExpensesDTO
                            {
                                Amount = i.Amount,
                                BudgetId = i.BudgetId,
                                Date_Withdraw = i.Date_Withdraw
                            }).ToList().AsReadOnly();

            return Result<IReadOnlyList<ExpensesDTO>>.Success(values);
        }

        private async Task notiSender(IEnumerable<string> IDlist, string message)
        {
            await _notificationServices.addNotificationscommon(IDlist, "Budget Title", message);
            var dispatchTasks = new List<Task>();
            foreach (var item in IDlist)
            {
                dispatchTasks.Add(signalR.SendMessageUser(item, message));
            }
            await Task.WhenAll(dispatchTasks);
        }
    }
}
