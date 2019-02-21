using System;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Decimal(10, 1)]
        public decimal Height { get; set; }

        [Decimal(18, 3)]
        public decimal Weight { get; set; }
    }
}
