using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations.Test.Models;
using Xunit;

namespace Toolbelt.ComponentModel.DataAnnotations.Test
{
    public class DecimalAttributeTest : IDisposable
    {
        private string DbName { get; }

        private SqlConnection ConnToMaster { get; }

        public DecimalAttributeTest()
        {
            DbName = Guid.NewGuid().ToString("N");

            const string connStrToMaster = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=True;";
            ConnToMaster = new SqlConnection(connStrToMaster);
            ConnToMaster.Open();

            Helper.ExecuteQueryToMaster(ConnToMaster, $"CREATE DATABASE [{DbName}]");
        }

        public void Dispose()
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var dataPhysicalPath = Path.Combine(baseDir, $"{DbName}.mdf");
            var logPhysicalPath = Path.Combine(baseDir, $"{DbName}_log.ldf");

            if (File.Exists(dataPhysicalPath) || File.Exists(logPhysicalPath))
            {
                Helper.ExecuteQueryToMaster(ConnToMaster, $@"
                    ALTER database [{DbName}] set offline with ROLLBACK IMMEDIATE;
                    DROP DATABASE [{DbName}]");
            }
            if (File.Exists(dataPhysicalPath)) File.Delete(dataPhysicalPath);
            if (File.Exists(logPhysicalPath)) File.Delete(logPhysicalPath);
            ConnToMaster.Dispose();
        }

        [Fact(DisplayName = "CreateDb with Decimal Column Type on MSSQL LocalDb")]
        public void CreateDb_with_DecimalAttribute_Test()
        {
            var connStr = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True;MultipleActiveResultSets=True;";
            var option = new DbContextOptionsBuilder<MyDbContext>().UseSqlServer(connStr).Options;
            using (var db = new MyDbContext(option))
            {
                // Create database.
                db.Database.OpenConnection();
                db.Database.EnsureCreated();

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
                        "People|Metric_Height_Value|decimal|18|3|False",
                        "People|Metric_Weight_Value|decimal|18|3|False"
                    );
                }
            }
        }

        private static class Helper
        {
            internal static void ExecuteQueryToMaster(SqlConnection conn, string sql)
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
