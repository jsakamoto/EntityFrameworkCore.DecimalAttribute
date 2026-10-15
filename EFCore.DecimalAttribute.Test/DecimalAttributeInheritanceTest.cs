using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations.Test.Models.Inheritance;
using Xunit;

namespace Toolbelt.ComponentModel.DataAnnotations.Test;

[Collection(nameof(SqlServerCollection))]
public class DecimalAttributeInheritanceTest
{
    private readonly SqlServerFixture _sqlServer;

    public DecimalAttributeInheritanceTest(SqlServerFixture sqlServer)
    {
        this._sqlServer = sqlServer;
    }

    [Fact(DisplayName = "[Decimal] on a base class property with TPH on SQL Server")]
    public void Tph_Test()
    {
        var options = this.CreateOptions();
        this.ValidateColumns(new TphDbContext(options),
            "Assets|Amount|18|3",
            "Assets|Mileage|10|1");
    }

    [Fact(DisplayName = "[Decimal] on a base class property with TPT on SQL Server")]
    public void Tpt_Test()
    {
        var options = this.CreateOptions();
        this.ValidateColumns(new TptDbContext(options),
            "Assets|Amount|18|3",
            "Vehicles|Mileage|10|1");
    }

    [Fact(DisplayName = "[Decimal] on a base class property with TPC on SQL Server")]
    public void Tpc_Test()
    {
        var options = this.CreateOptions();
        this.ValidateColumns(new TpcDbContext(options),
            "Assets|Amount|18|3",
            "Furnitures|Amount|18|3",
            "Vehicles|Amount|18|3",
            "Vehicles|Mileage|10|1");
    }

    private DbContextOptions CreateOptions()
        => new DbContextOptionsBuilder().UseSqlServer(this._sqlServer.CreateDatabase()).Options;

    private void ValidateColumns(InheritanceDbContext db, params string[] expected)
    {
        using var _ = db;

        db.Database.OpenConnection();
        db.Database.EnsureCreated();
        try
        {
            var conn = db.Database.GetDbConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT TABLE_NAME, COLUMN_NAME, NUMERIC_PRECISION, NUMERIC_SCALE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE COLUMN_NAME IN ('Amount', 'Mileage')";
            var dump = new List<string>();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read()) dump.Add($"{r[0]}|{r[1]}|{r[2]}|{r[3]}");
            }
            dump.Order(StringComparer.Ordinal).Is(expected);
        }
        finally { db.Database.EnsureDeleted(); }
    }
}
