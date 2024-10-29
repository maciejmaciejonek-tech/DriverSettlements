using DriverSettlements.Common.Models;
using DriverSettlements.Database.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        public DbSet<BillingWeek> BillingWeeks { get; set; }
        public DbSet<TariffPlanAssignment> TariffPlanAssignments { get; set; }
        public DbSet<TariffPlan> TariffPlans { get; set; }
        public DbSet<DefReport> DefReports { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<ExternalReport> ExternalReports { get; set; }
        public DbSet<FuelBill> FuelBills { get; set; }
        public DbSet<SettlementItem> SettlementItems { get; set; }
        public DbSet<SettlementType> SettlementTypes { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        private string _dbPath { get; }

        public DatabaseContext(IOptions<DatabaseConfiguration> databaseConfiguration)
        {
            _dbPath = databaseConfiguration.Value.Path; 
            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_dbPath}");
    }
}
