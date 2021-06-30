using Microsoft.EntityFrameworkCore;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

#nullable disable
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
#nullable enable

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var person = modelBuilder.Entity<Person>();
            person.OwnsOne(p => p.Metric, metric =>
            {
                metric.OwnsOne(m => m.Height);
                metric.OwnsOne(m => m.Weight);
            });

            //modelBuilder.Entity<Person>().OwnsOne(typeof(Metric), "Metric", metric =>
            //{
            //    metric.OwnsOne(typeof(Mesure), "Height", m => m.Property("Value").HasColumnType("decimal(18, 3)"));
            //    metric.OwnsOne(typeof(Mesure), "Weight", m => m.Property("Value").HasColumnType("decimal(18, 3)"));
            //});
            //modelBuilder.Entity<Person>().Property("EyeSight").HasColumnType("decimal(10, 1)");

            modelBuilder.BuildDecimalColumnTypeFromAnnotations();
        }
    }
}
