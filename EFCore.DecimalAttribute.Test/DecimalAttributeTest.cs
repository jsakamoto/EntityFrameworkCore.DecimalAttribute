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
            var dbName = Guid.NewGuid().ToString("N");

            using (var connToMaster = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;"))
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

            var connStr = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=True;";
            var option = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlServer(connStr)
                .UseLoggerFactory(loggerFactory)
                .Options;

            return new MyDbContext(option);
        }

        [Fact(DisplayName = "CreateDb with Decimal Column Type on MSSQL LocalDb")]
        public void CreateDb_with_DecimalAttribute_Test()
        {
            using (var db = CreateMyDbContext())
            {
                // Create database.
                db.Database.OpenConnection();
                db.Database.EnsureCreated();

                try
                {
                    // Validate database column types.
                    var conn = db.Database.GetDbConnection() as SqlConnection;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT [Table] = t.name, [Column] = c.name, [Type] = type.name, [Precision] = c.precision, [Scale] = c.scale, [Nullable] = c.is_nullable
                        FROM sys.tables t
                        INNER JOIN sys.columns c ON t.object_id = c.object_id
                        INNER JOIN sys.types type ON c.system_type_id = type.system_type_id
                        WHERE t.name = 'People'
                        ORDER BY t.name, c.name";
                        var dump = new List<string>();
                        var r = cmd.ExecuteReader();
                        try { while (r.Read()) dump.Add($"{r["Table"]}|{r["Column"]}|{r["Type"]}|{r["Precision"]}|{r["Scale"]}|{r["Nullable"]}"); }
                        finally { r.Close(); }
                        dump.Is(
                            "People|EyeSight|decimal|10|1|False",
                            "People|Id|int|10|0|False",
                            // NOTICE: Owned Property in EF Core v3 never be non-nullable.
                            // See also: https://github.com/aspnet/EntityFrameworkCore/issues/16943
                            "People|Metric_Height_Value|decimal|18|3|True",
                            "People|Metric_Weight_Value|decimal|18|3|True"
                        );
                    }
                }
                finally { db.Database.EnsureDeleted(); }
            }
        }
    }
}
