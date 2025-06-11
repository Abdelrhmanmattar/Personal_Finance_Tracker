using Core.DTO;
using Core.entities;
using Core.Interfaces;
using Core.Result;
using Core.Specification;
using Services.caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class ReportServices:IReport
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IcacheServices _cache;
        private readonly IMainUser_Repo mainUser_;

        public ReportServices(IUnitOfWork _unitOfWork , IcacheServices cache , IMainUser_Repo mainUser_)
        {
            unitOfWork = _unitOfWork;
            _cache = cache;
            this.mainUser_ = mainUser_;
        }
        public async Task<Result<ReportSummaryDTO>> Summary(ClaimsPrincipal claims, DateTime? start=null , DateTime? end=null)
        {
            var user = await mainUser_.GetCurrentUser(claims);
            if (user == null)
                return Result<ReportSummaryDTO>.Fail(null, "error authorized");


            string ID = user.Id;
            //income data specs,list , total,top
            var specIncome = new BaseSpecification<Income>(i =>
            (
                i.User_Id == ID &&
                (!start.HasValue || i.Date_Deposite >= start.Value) &&
                (!end.HasValue || i.Date_Deposite <= end.Value)
            ));
            IEnumerable<IncomeDTO> Incomes = (await _cache.GetAllfromCache<IncomeDTO>(ID)) ??
                unitOfWork.Repository<Income>().FindAll(specIncome).Select(
                    i => new IncomeDTO
                    {
                        Id = i.Id,
                        Amount = i.Amount,
                        Date_Deposite = i.Date_Deposite,
                        Source = i.Source
                    }).
                    ToList();
            var totalIncome = Incomes.Sum(i => i.Amount);
            var maxIncome = Incomes.Max(i => i.Amount);
            var topIncome = Incomes.Where(i => i.Amount == maxIncome).ToList().AsReadOnly();


            //Expenses data specs,list , total,top
            var specExpenses = new BaseSpecification<Expenses>(i => 
            (
                i.User_Id == ID&&
                (!start.HasValue || i.Date_Withdraw >= start.Value) &&
                (!end.HasValue || i.Date_Withdraw <= end.Value)
            ));
            IEnumerable<ExpensesDTO> Expenses = await _cache.GetAllfromCache<ExpensesDTO>(ID) ??
                                unitOfWork.Repository<Expenses>().FindAll(specExpenses).Select(
                    i => new ExpensesDTO
                    {
                        Id = i.Id,
                        Amount = i.Amount,
                        Date_Withdraw = i.Date_Withdraw,
                        BudgetId = i.BudgetId
                    }).
                    ToList();
            var totalExpenses = Expenses.Sum(e => e.Amount);
            var maxExpenses = Expenses.Max(e => e.Amount);
            var topExpenses = Expenses.Where(e => e.Amount == maxExpenses).ToList().AsReadOnly();
            var expensesInCategory = Expenses.GroupBy(e => e.BudgetId)
                         .Select(s => new
                         {
                             CatID = s.Key,
                             totalSpend = s.Sum(e => e.Amount)
                         }).ToList().AsReadOnly();




            var specBudgets = new BaseSpecification<Budgets>(i => (i.User_Id == ID));

            IEnumerable<BudgetDTO> Budgets = await _cache.GetAllfromCache<BudgetDTO>(ID) ??
                             unitOfWork.Repository<Budgets>().FindAll(specBudgets).Select(
                    i => new BudgetDTO
                    {
                        ID = i.Id,
                        Cat_Id = i.Cat_Id,
                        LimitAmount = i.LimitAmount,
                    }).
                    ToList();

            string status = "";

            foreach(var Budget in Budgets)
            {
                var spend = expensesInCategory.FirstOrDefault(e => e.CatID == Budget.Cat_Id)?.totalSpend ?? 0;
                if (spend > Budget.LimitAmount)
                    status += "Exceeded,";
                else if (spend >= (0.85m * Budget.LimitAmount))
                    status += "Near,";
                else
                    status += "Under,";
            }
            status = status.Remove(status.Length - 1);


            var report = new ReportSummaryDTO()
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                Balance = totalIncome - totalExpenses,
                TopIncomeSource = string.Join(",", topIncome),
                TopExpenseCategory = string.Join(",", topExpenses),
                BudgetStatus = status
            };
            return Result<ReportSummaryDTO>.Success(report);
        }
    }
}
