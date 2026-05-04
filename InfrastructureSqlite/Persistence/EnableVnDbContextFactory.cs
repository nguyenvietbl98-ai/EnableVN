using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureSqlite.Persistence
{
    /// <summary>
    /// Factory cho EF Core migrations.
    /// 
    /// Dùng khi chạy lệnh:
    /// dotnet ef migrations add <MigrationName> --project InfrastructureSqlite
    /// dotnet ef database update --project InfrastructureSqlite
    /// 
    /// Nó cho phép dotnet ef tools khởi tạo DbContext mà không cần chạy Program.cs.
    /// </summary>
    public sealed class EnableVnDbContextFactory : IDesignTimeDbContextFactory<EnableVnDbContext>
    {
        public EnableVnDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EnableVnDbContext>();

            // Default connection string nếu chưa cấu hình
            var connectionString = "Data Source=enablevn.db";

            optionsBuilder.UseSqlite(connectionString);

            return new EnableVnDbContext(optionsBuilder.Options);
        }
    }
}
