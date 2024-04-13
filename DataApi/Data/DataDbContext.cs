using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using DataApi.Models;
using System;

namespace DataApi.Data
{
    public class DataDbContext:DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
        }

        public DbSet<Currency> Currency { get; set; }
    }
}