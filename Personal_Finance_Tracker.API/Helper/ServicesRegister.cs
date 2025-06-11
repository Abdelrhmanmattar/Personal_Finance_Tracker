using Core.entities;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.MODELS.DATA;
using Repository.Repositories;
using Repository.UnitofWork;
using Services.BackgroundServices;
using Services.caching;
using Services.Service;
using StackExchange.Redis;
using System.Text;

namespace Personal_Finance_Tracker.API.Helper
{
    public static class ServicesRegister
    {
        public static IServiceCollection addDbServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddDbContext<DBcontext>
            (
                options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("constr"),
                        b => b.MigrationsAssembly(typeof(DBcontext).Assembly.FullName));
                }
            );
            Services.AddStackExchangeRedisCache(redisoptions =>
            {
                redisoptions.Configuration = Configuration.GetConnectionString("redis");
                redisoptions.InstanceName = "Tracker_";

            });
            Services.AddSingleton<IConnectionMultiplexer>(re =>
            {
                return ConnectionMultiplexer.Connect(Configuration.GetConnectionString("redis"));
            });

            return Services;
        }

        public static IServiceCollection addAuth(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddIdentity<Users_App, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            }
            ).AddEntityFrameworkStores<DBcontext>();


            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });
            return Services;
        }

        public static IServiceCollection addCustomServices(this IServiceCollection Services)
        {
            Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen();

            Services.AddSignalR();
            Services.AddScoped(typeof(ISpecification<>), typeof(BaseSpecification<>));

            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
            Services.AddScoped<IMainUser_Repo, MainUser_Repo>();
            Services.AddScoped<ISignalRService, SignalRService>();
            Services.AddScoped<IExpense, ExpenseServices>();
            Services.AddScoped<ICategory, ClategoryServices>();
            Services.AddScoped<IBudget, BudgetServices>();
            Services.AddScoped<IIncome, InComeServices>();
            Services.AddScoped<IReport, ReportServices>();
            Services.AddScoped<IBudgetUser, BudgetUserServices>();

            Services.AddSingleton<IcacheServices, CacheServices>();

            Services.AddScoped<INotificationServices, NotificationServices>();
            Services.AddHostedService<CleanNotification>();

            return Services;
        }

    }
}
