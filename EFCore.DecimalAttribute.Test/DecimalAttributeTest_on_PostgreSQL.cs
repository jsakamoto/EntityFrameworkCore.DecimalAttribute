using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations.Test.Models;
using Xunit;

namespace Toolbelt.ComponentModel.DataAnnotations.Test;

[Collection(nameof(PostgreSqlCollection))]
public class DecimalAttributeTest_on_PostgreSQL
{
    private readonly PostgreSqlFixture _postgreSql;

    public DecimalAttributeTest_on_PostgreSQL(PostgreSqlFixture postgreSql)
    {
        this._postgreSql = postgreSql;
    }

    [Fact(DisplayName = "CreateDb with Decimal Column Type on PostgreSQL")]
    public void CreateDb_with_DecimalAttribute_Test()
    {
        var option = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(this._postgreSql.CreateDatabase())
            .Options;
        using var db = new MyDbContext(option);

        // Create database.
        db.Database.OpenConnection();
        db.Database.EnsureCreated();

        try
        {
            // Validate database column types.
            var conn = db.Database.GetDbConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                        SELECT table_name, column_name, data_type, numeric_precision, numeric_scale, is_nullable
                        FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'People'
                        ORDER BY column_name COLLATE ""C""";
            var dump = new List<string>();
            var r = cmd.ExecuteReader();
            try { while (r.Read()) dump.Add($"{r[0]}|{r[1]}|{r[2]}|{r[3]}|{r[4]}|{r[5]}"); }
            finally { r.Close(); }
            dump.Is(
                "People|EyeSight|numeric|10|1|NO",
                "People|Id|integer|32|0|NO",
                "People|Metric_Height_Value|numeric|18|3|NO",
                "People|Metric_Weight_Value|numeric|18|3|NO"
            );
        }
        finally
        {
            // PostgreSQL can't drop a database while a connection to it is open.
            db.Database.CloseConnection();
            db.Database.EnsureDeleted();
        }
    }
}
