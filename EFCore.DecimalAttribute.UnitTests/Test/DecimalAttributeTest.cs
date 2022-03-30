using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Toolbelt.ComponentModel.DataAnnotations.Test.Models;
using Xunit;

namespace Toolbelt.ComponentModel.DataAnnotations.Test
{
    public class DecimalAttributeTest
    {
        private static MyDbContext CreateMyDbContext()
        {
            var server = Environment.GetEnvironmentVariable("MSSQL_SERVER");
            if (string.IsNullOrEmpty(server)) server = "(localdb)\\mssqllocaldb";
            var user = Environment.GetEnvironmentVariable("MSSQL_USER");
            var pwd = Environment.GetEnvironmentVariable("MSSQL_PWD");
            var credential = (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pwd)) ? $"User={user};Password={pwd}" : "Integrated Security=True";
            var connStrBase = $"Server={server};{credential};TrustServerCertificate=True;";

            var dbName = Guid.NewGuid().ToString("N");

            using (var connToMaster = new SqlConnection(connStrBase + "Database=master;"))
            using (var cmd = new SqlCommand($"CREATE DATABASE [{dbName}]", connToMaster))
            {
                connToMaster.Open();
                cmd.ExecuteNonQuery();
            }

            var loggerFactory = new LoggerFactory(
                new[] { new DebugLoggerProvider() },
                new LoggerFilterOptions
                {
                    Rules = { new LoggerFilterRule("Debug", DbLoggerCategory.Database.Command.Name, LogLevel.Information, (n, c, l) => true) }
                });

            var connStr = connStrBase + $"Database={dbName};";
            var option = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlServer(connStr)
                .UseLoggerFactory(loggerFactory)
                .Options;

            return new MyDbContext(option);
        }

        [Fact(DisplayName = "CreateDb with Decimal Column Type on MSSQL LocalDb")]
        public void CreateDb_with_DecimalAttribute_Test()
        {
            // Create database.
            using var db = CreateMyDbContext();
            db.Database.OpenConnection();
            db.Database.EnsureCreated();

            // NOTICE: Owned Property in EF Core v3 never be non-nullable.
            // See also: https://github.com/aspnet/EntityFrameworkCore/issues/16943
            var nullableOwnedTypes =
#if ENABLE_NON_NULLABLE_OWNED_TYPES
                false;
#else
                true;
#endif
            try
            {
                // Validate database column types.
                var dump = DumpColumnsTypes(db);
                dump.Is(
                    "People|EyeSight|decimal|10|1|False",
                    "People|Id|int|10|0|False",
                    $"People|Metric_Height_Value|decimal|18|3|{nullableOwnedTypes}",
                    $"People|Metric_Weight_Value|decimal|18|3|{nullableOwnedTypes}"
                );
            }
            finally { db.Database.EnsureDeleted(); }
        }

        private static List<string> DumpColumnsTypes(MyDbContext db)
        {
            var dump = new List<string>();
            var conn = db.Database.GetDbConnection() as SqlConnection;
#nullable disable
            using var cmd = conn.CreateCommand();
#nullable enable
            cmd.CommandText = @"
                        SELECT [Table] = t.name, [Column] = c.name, [Type] = type.name, [Precision] = c.precision, [Scale] = c.scale, [Nullable] = c.is_nullable
                        FROM sys.tables t
                        INNER JOIN sys.columns c ON t.object_id = c.object_id
                        INNER JOIN sys.types type ON c.system_type_id = type.system_type_id
                        WHERE t.name = 'People'
                        ORDER BY t.name, c.name";
            var r = cmd.ExecuteReader();
            try { while (r.Read()) dump.Add($"{r["Table"]}|{r["Column"]}|{r["Type"]}|{r["Precision"]}|{r["Scale"]}|{r["Nullable"]}"); }
            finally { r.Close(); }
            return dump;
        }
    }
}
