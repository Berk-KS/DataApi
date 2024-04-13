
using DataApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DataApi
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<DataDbContext>
    {
        public DataDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<DataDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
            builder.UseMySql(connectionString, serverVersion);

            return new DataDbContext(builder.Options);
        }


    }
}
