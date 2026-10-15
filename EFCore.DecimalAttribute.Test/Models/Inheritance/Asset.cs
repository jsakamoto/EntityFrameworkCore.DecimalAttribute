namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models.Inheritance;

public class Asset : MeasuredBase
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
}
