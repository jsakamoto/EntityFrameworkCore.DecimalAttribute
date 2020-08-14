using System;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models
{
    public class Mesure
    {
        [Decimal(18, 3)]
        public decimal Value { get; set; }
    }
}
