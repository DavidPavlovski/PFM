using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PFM.DataAccess.DbContextOption;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.Repositories.Repository;
using PFM.DataAccess.UnitOfWork;
using PFM.Mapping.AutoMapper;
using PFM.Services.Abstraction;
using PFM.Services.Implementation;
using PFM.Validations.Category;
using PFM.Validations.Split;

namespace PFM.IOC
{
    public static class IoCContainer
    {
        public static IServiceCollection RegisterModule(this IServiceCollection services, string connectionString)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionSplitRepository, TransactionSplitRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<ICategoryValidator, CategoryValidator>();
            services.AddScoped<ISplitValidator, SplitValidator>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
