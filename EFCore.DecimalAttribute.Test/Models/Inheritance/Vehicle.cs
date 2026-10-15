using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models.Inheritance;

public class Vehicle : Asset
{
    [Decimal(10, 1)]
    public decimal Mileage { get; set; }
}
