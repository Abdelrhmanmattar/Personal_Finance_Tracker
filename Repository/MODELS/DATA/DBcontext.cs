using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Core.entities;
using Microsoft.Extensions.Configuration;
using Repository.MODELS.CONFIG;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;


namespace Repository.MODELS.DATA
{
    public class DBcontext : IdentityDbContext<Users_App>
    {
        public DBcontext()
        {

        }
        public DBcontext(DbContextOptions options) : base(options)
        {

            try
            {
                var dbcreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if(dbcreator!= null)
                {
                    if (!dbcreator.CanConnect())
                        dbcreator.Create();
                    if(!dbcreator.HasTables())
                        dbcreator.CreateTables();
                }

            }
            catch(Exception x) {
                Console.WriteLine(x.Message);
            }
        }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budgets> Budgets { get; set; }
        public DbSet<BudgetUser> BudgetUsers { get; set; }
        public DbSet<Notifications> Notifications { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserApp_Config());
            modelBuilder.ApplyConfiguration(new IncomeConfig());
            modelBuilder.ApplyConfiguration(new Category_Config());
            modelBuilder.ApplyConfiguration(new BudgetsConfig());
            modelBuilder.ApplyConfiguration(new Expenses_Config());
            modelBuilder.ApplyConfiguration(new NotificationsConfig());
            modelBuilder.ApplyConfiguration(new Category_Config());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
/*             var baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

            var ApiProject = Path.Combine(baseDirectory, "Personal_Finance_Tracker.API");
            var ApiSettings = Path.Combine(ApiProject, "appsettings.json");


            var config = new ConfigurationBuilder()
                .AddJsonFile(ApiSettings).Build();

            var constr = config.GetConnectionString("constr");
            optionsBuilder.UseSqlServer(constr);
 */
        }
    }
}
