using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models.Inheritance;

/// <summary>
/// A base class that is not an entity type by itself. Its <c>[Decimal]</c> property is inherited by the entity types below.
/// </summary>
public abstract class MeasuredBase
{
    [Decimal(18, 3)]
    public decimal Amount { get; set; }
}
