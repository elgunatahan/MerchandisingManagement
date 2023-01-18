using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence;

namespace Api.Configuration
{
    public class SqlBootstrapper : MerchandisingManagementContext
    {
        public SqlBootstrapper(DbContextOptions<MerchandisingManagementContext> options) : base(options)
        {

        }

        public void Migrate()
        {
            RelationalDatabaseCreator databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }
    }
}
