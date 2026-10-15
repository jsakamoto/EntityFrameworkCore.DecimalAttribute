using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Toolbelt.ComponentModel.DataAnnotations.Test.Models;
using Xunit;

namespace Toolbelt.ComponentModel.DataAnnotations.Test;

[Collection(nameof(SqlServerCollection))]
public class DecimalAttributeTest
{
    private readonly SqlServerFixture _sqlServer;

    public DecimalAttributeTest(SqlServerFixture sqlServer)
    {
        this._sqlServer = sqlServer;
    }

    private MyDbContext CreateMyDbContext()
    {
        var connStr = this._sqlServer.CreateDatabase();

        var loggerFactory = new LoggerFactory(
            new[] { new DebugLoggerProvider() },
            new LoggerFilterOptions
            {
                Rules = { new LoggerFilterRule("Debug", DbLoggerCategory.Database.Command.Name, LogLevel.Information, (n, c, l) => true) }
            });

        var option = new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlServer(connStr)
            .UseLoggerFactory(loggerFactory)
            .Options;

        return new MyDbContext(option);
    }

    [Fact(DisplayName = "CreateDb with Decimal Column Type on SQL Server")]
    public void CreateDb_with_DecimalAttribute_Test()
    {
        // Create database.
        using var db = this.CreateMyDbContext();
        db.Database.OpenConnection();
        db.Database.EnsureCreated();

        try
        {
            // Validate database column types.
            var dump = DumpColumnsTypes(db);
            dump.Is(
                "People|EyeSight|decimal|10|1|False",
                "People|Id|int|10|0|False",
                $"People|Metric_Height_Value|decimal|18|3|False",
                $"People|Metric_Weight_Value|decimal|18|3|False"
            );
        }
        finally { db.Database.EnsureDeleted(); }
    }

    private static List<string> DumpColumnsTypes(MyDbContext db)
    {
        var dump = new List<string>();
        var conn = db.Database.GetDbConnection() as SqlConnection ?? throw new Exception("The connection object is not SqlConnection");
        using var cmd = conn.CreateCommand();
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
