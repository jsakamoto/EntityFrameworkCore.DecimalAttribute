# DecimalAttribute for EntityFramework Core  
[![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.EntityFrameworkCore.DecimalAttribute.svg)](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute/)

## What's this?

This class library for Entity Framework Core on .NET Core/.NET Framework provides the ability for you to control the **precision** and **scale** of decimal column type, **by annotating** properties of entity types, like this.

```csharp
[Decimal(10, 5)] // <-- This!
public decimal Height { get; set; }
```

## How to use?

1. Add [`Toolbelt.EntityFrameworkCore.DecimalAttribute`](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute/) package to your project.

```shell
> dotnet add package Toolbelt.EntityFrameworkCore.DecimalAttribute
```
### Supported Versions

EF Core version | This package version
----------------|-------------------------
v.3.0           | v.3.0
v.2.0, 2.1, 2.2 | v.1.0.x

2. Annotate your model with `[Decimal(precision, scale)]` attribute that lives in `Toolbelt.ComponentModel.DataAnnotations.Schema` namespace.

```csharp
using Toolbelt.ComponentModel.DataAnnotations.Schema;

public class Person
{
    public int Id { get; set; }

    [Decimal(10, 5)] // <- Here!
    public decimal Height { get; set; }
}
```

3. **[Important]** Override `OnModelCreating()` method of your DbContext class, and call `BuildDecimalColumnTypeFromAnnotations()` extension method which lives in `Toolbelt.ComponentModel.DataAnnotations` namespace.

```csharp
using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations;

public class MyDbContext : DbContext
{
    ...
    // Override "OnModelCreating", ...
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // .. and invoke "BuildDecimalColumnTypeFromAnnotations"!
        modelBuilder.BuildDecimalColumnTypeFromAnnotations();
    }
}
```

That's all!

`BuildDecimalColumnTypeFromAnnotations()` extension method scans the DbContext with .NET Reflection technology, and detects `[Decimal]` attributes, then build models related to decimal column type.

After doing that, decimal type columns in the database which created by EF Core, is configured with precisions and scales that are specified by `[Decimal]` attribute.

## Appendix

### If you want to use only "DecimalAttribute" without any dependencies...

If you want to use only "DecimalAttribute" class without any dependencies, you can use [Toolbelt.EntityFrameworkCore.DecimalAttribute.Attribute](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute.Attribute) NuGet package.

## Release Note

- **v.3.0.0** - BREAKING CHANGE: supports EntityFramework Core v.3.0
- **v.1.0.1**
  - Fix: Doesn't work with owned types on EFCore v.2.1, v.2.2.
  - Fix: Doesn't work with nested owned types.
- **v.1.0.0** - 1st release.


## License

[MIT License](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/blob/master/LICENSE)

