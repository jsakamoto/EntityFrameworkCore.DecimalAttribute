﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //var person = modelBuilder.Entity<Person>();
            //person.OwnsOne(p => p.Address, address =>
            //{
            //    address.OwnsOne(a => a.Lines,
            //        lines =>
            //        {
            //            lines.Property(l => l.Line1).HasColumnName("Line1");
            //            lines.Property(l => l.Line2).HasColumnName("Line2");
            //            lines.Property(l => l.Line3).HasColumnName("Line3");
            //        });
            //});
            //person.OwnsOne(p => p.PhoneNumber);
            //person.OwnsOne(p => p.FaxNumber);

            //modelBuilder.Entity<Person>().OwnsOne(typeof(Address), "Address", address =>
            //{
            //    address.OwnsOne(typeof(Lines), "Lines", l => l.HasIndex("Line1", "Line2"));
            //});
            //modelBuilder.Entity<Person>().HasIndex("Name").IsUnique();
            //modelBuilder.Entity<SNSAccount>().HasIndex("Provider");
            //modelBuilder.Entity<SNSAccount>().HasIndex("Provider", "AccountName").HasName("Ix_Provider_and_Account").IsUnique();

            modelBuilder.BuildDecimalColumnTypeFromAnnotations();
        }

    }
}
