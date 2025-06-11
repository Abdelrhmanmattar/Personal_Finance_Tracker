using Core.DTO;
using Core.entities;
using Core.Interfaces;
using Core.Result;
using Core.Specification;
using Services.caching;
using System.Security.Claims;

namespace Services.Service
{
    public class BudgetUserServices : IBudgetUser
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISignalRService _signalR;
        private readonly IcacheServices _cache;
        private readonly INotificationServices _notificationServices;
        private readonly IMainUser_Repo mainUser_;

        public BudgetUserServices(
            IUnitOfWork unitOfWork, ISignalRService signalR,
            IcacheServices cache, INotificationServices notificationServices,
            IMainUser_Repo mainUser_
            )
        {
            _unitOfWork = unitOfWork;
            _signalR = signalR;
            _cache = cache;
            _notificationServices = notificationServices;
            this.mainUser_ = mainUser_;
        }

        public async Task<Result<BudgetUserDTO>> AddUserToBudget(ClaimsPrincipal claims, string email, int idBudget)
        {

            var userWatcher = await mainUser_.GetUserByEmail(email);
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null || userWatcher == null)
            {
                return Result<BudgetUserDTO>.Fail(null, "Error on Email or Watcher");
            }
            string ID = user.Id;

            var specs = new BaseSpecification<Budgets>(
                b => (b.Id == idBudget && b.User_Id == ID)
                );
            var budget = _unitOfWork.Repository<Budgets>().Find(specs);

            if (budget != null)
            {
                var budgetUser = new BudgetUser()
                {
                    UserId = userWatcher,
                    BudgetId = budget.Id,
                    Role = "Watcher"
                };

                var res = _unitOfWork.Repository<BudgetUser>().AddEntity(budgetUser);
                _unitOfWork.SaveChanges();
                await UserNotifaction(userWatcher, $"You have been added to the budget {budget.Id} as a watcher from {user.Email}.");

                return (res == true) ?
                    Result<BudgetUserDTO>.Success(new BudgetUserDTO() { Id = budgetUser.Id, BudgetId = budgetUser.BudgetId, Role = budgetUser.Role })
                    :
                    Result<BudgetUserDTO>.Fail(null, "Error on add");
            }
            return Result<BudgetUserDTO>.Fail(null, "Error on Email or Budget");
        }
        public async Task<Result<BudgetUserDTO>> RemoveUserFromBudget(ClaimsPrincipal claims, string email, int idBudget)
        {
            var userWatcher = await mainUser_.GetUserByEmail(email);
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null || userWatcher == null)
            {
                return Result<BudgetUserDTO>.Fail(null, "Error on Email or Watcher");
            }
            string ID = user.Id;

            var specs = new BaseSpecification<Budgets>(
                b => (b.Id == idBudget && b.User_Id == ID)
                );
            var budget = _unitOfWork.Repository<Budgets>().Find(specs);

            if (budget != null)
            {
                var specsBudgetUser = new BaseSpecification<BudgetUser>(
                    b => (b.BudgetId == idBudget && b.UserId == userWatcher && b.Role == "Watcher")
                    );

                var entityUser = _unitOfWork.Repository<BudgetUser>().Find(specsBudgetUser);

                if (entityUser != null)
                {
                    _unitOfWork.Repository<BudgetUser>().DeleteEntity(entityUser.Id);
                    _unitOfWork.SaveChanges();
                    await UserNotifaction(userWatcher, $"You have been removed from the budget {budget.Id} as a watcher by {user.Email}.");
                    return Result<BudgetUserDTO>.Success(null);
                }
            }
            return Result<BudgetUserDTO>.Fail(null, "Error on Email or Budget");
        }
        public async Task<Result<IReadOnlyList<User_appDTO>>> UsersSubscribeInAdminBudget(ClaimsPrincipal claims, int idBudget)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
            {
                return Result<IReadOnlyList<User_appDTO>>.Fail(null, "Error not allowed");
            }
            string ID = user.Id;
            var specsBudget = new BaseSpecification<Budgets>(b => b.Id == idBudget && b.User_Id == ID);
            var budget = _unitOfWork.Repository<Budgets>().Find(specsBudget);
            // Check if the budget exists and belongs to the current user
            if (budget == null)
            {
                return Result<IReadOnlyList<User_appDTO>>.Fail(null, "Not Found");
            }

            var specs = new BaseSpecification<BudgetUser>(b => b.BudgetId == idBudget);
            specs.AddInclude(b => b.User);
            specs.AddInclude(b => b.Budget);

            var values = _unitOfWork.Repository<BudgetUser>().FindAll(specs);

            if (values == null)
            {
                return Result<IReadOnlyList<User_appDTO>>.Fail(null, "Not Found");
            }
            var users = values.Select(u => new User_appDTO
            {
                Name = u.User?.UserName,
                Email = u.User?.Email,
            }).ToList().AsReadOnly();
            return Result<IReadOnlyList<User_appDTO>>.Success(users);
        }
        public async Task<Result<IEnumerable<string>>> IDsSubscribeInAdminBudget(ClaimsPrincipal claims, int idBudget)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
            {
                return Result<IEnumerable<string>>.Fail(null, "Error not allowed");
            }
            string ID = user.Id;
            var specsBudget = new BaseSpecification<Budgets>(b => b.Id == idBudget && b.User_Id == ID);
            var budget = _unitOfWork.Repository<Budgets>().Find(specsBudget);
            // Check if the budget exists and belongs to the current user
            if (budget == null)
            {
                return Result<IEnumerable<string>>.Fail(null, "Not Found");
            }

            var specs = new BaseSpecification<BudgetUser>(b => b.BudgetId == idBudget);

            var values = _unitOfWork.Repository<BudgetUser>().FindAll(specs);

            if (values == null)
            {
                return Result<IEnumerable<string>>.Fail(null, "Not Found");
            }
            var users = values.Select(u => u.UserId).ToList().AsReadOnly();
            return Result<IEnumerable<string>>.Success(users);
        }
        public async Task<Result<IReadOnlyList<WatcherTreeDTO>>> MyBudgetsAsWatcher(ClaimsPrincipal claims)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
            {
                return Result<IReadOnlyList<WatcherTreeDTO>>.Fail(null, "Error not allowed");
            }
            string ID = user.Id;

            var specs = new BaseSpecification<BudgetUser>(b => b.UserId == ID);
            specs.AddInclude(b => b.Budget);
            specs.AddInclude(b => b.Budget.User_App); // This works for EF Core ≥5

            var values = _unitOfWork.Repository<BudgetUser>().FindAll(specs);
            if (values == null)
            {
                return Result<IReadOnlyList<WatcherTreeDTO>>.Fail(null, "Budgets Not Found");
            }
            var valuesIncludes = values
                .Select(v => new WatcherTreeDTO
                {
                    BudgetId = v.BudgetId,
                    LimitAmount = v.Budget?.LimitAmount ?? -1,
                    Emails = v.Budget?.User_App?.Email ?? "Unknown"
                }).ToList().AsReadOnly();
            return Result<IReadOnlyList<WatcherTreeDTO>>.Success(valuesIncludes);
        }

        private async Task UserNotifaction(string UserID, string Message = "")
        {
            await _signalR.SendMessageUser(UserID, Message);
            _notificationServices.AddNotification(new Notifications()
            {
                UserId = UserID,
                Title = "Budget Watcher",
                Message = Message,
                Type = "Watcher",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
