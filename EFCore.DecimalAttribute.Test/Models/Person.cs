using System;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Decimal(10, 1)]
        public decimal EyeSight { get; set; }

        public Metric Metric { get; set; }
    }
}
