using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations.Test.Models.Inheritance;
using Xunit;

namespace Toolbelt.ComponentModel.DataAnnotations.Test;

[Collection(nameof(PostgreSqlCollection))]
public class DecimalAttributeInheritanceTest_on_PostgreSQL
{
    private readonly PostgreSqlFixture _postgreSql;

    public DecimalAttributeInheritanceTest_on_PostgreSQL(PostgreSqlFixture postgreSql)
    {
        this._postgreSql = postgreSql;
    }

    [Fact(DisplayName = "[Decimal] on a base class property with TPH on PostgreSQL")]
    public void Tph_Test()
    {
        var options = this.CreateOptions();
        this.ValidateColumns(new TphDbContext(options),
            "Assets|Amount|18|3",
            "Assets|Mileage|10|1");
    }

    [Fact(DisplayName = "[Decimal] on a base class property with TPT on PostgreSQL")]
    public void Tpt_Test()
    {
        var options = this.CreateOptions();
        this.ValidateColumns(new TptDbContext(options),
            "Assets|Amount|18|3",
            "Vehicles|Mileage|10|1");
    }

    [Fact(DisplayName = "[Decimal] on a base class property with TPC on PostgreSQL")]
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
        => new DbContextOptionsBuilder().UseNpgsql(this._postgreSql.CreateDatabase()).Options;

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
                SELECT table_name, column_name, numeric_precision, numeric_scale
                FROM information_schema.columns
                WHERE table_schema = 'public' AND column_name IN ('Amount', 'Mileage')";
            var dump = new List<string>();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read()) dump.Add($"{r[0]}|{r[1]}|{r[2]}|{r[3]}");
            }
            dump.Order(StringComparer.Ordinal).Is(expected);
        }
        finally
        {
            // PostgreSQL can't drop a database while a connection to it is open.
            db.Database.CloseConnection();
            db.Database.EnsureDeleted();
        }
    }
}
