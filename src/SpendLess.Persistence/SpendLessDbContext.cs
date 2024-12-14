using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Persistence
{
    public class SpendLessDbContext : AuditableDbContext
    {
        public SpendLessDbContext(DbContextOptions<SpendLessDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpendLessDbContext).Assembly);
        }

    }
}
