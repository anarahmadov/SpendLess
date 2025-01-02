using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpendLess.Application.Contracts.Persistence;
using SpendLess.Application.Contracts.Persistence.Token;
using SpendLess.Persistence.Repositories;
using SpendLess.Persistence.Repositories.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SpendLessDbContext>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("SpendLessConnectionString")));

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITokenRepository, TokenRepository>();
            //services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            //services.AddScoped<ILeaveAllocationRepository, LeaveAllocationRepository>();

            return services;
        }
    }
}
