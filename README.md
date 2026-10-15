# DecimalAttribute for Entity Framework Core  
[![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.EntityFrameworkCore.DecimalAttribute.svg)](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute/) [![unit tests](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/actions/workflows/unit-tests.yml/badge.svg?branch=master&event=push)](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/actions/workflows/unit-tests.yml)

## What's this?

This library for Entity Framework Core lets you set the **precision** and **scale** of decimal columns **by annotating** the properties of your entity types, like this.

```csharp
[Decimal(10, 5)] // <-- This!
public decimal Height { get; set; }
```

The automated tests check that it works on **SQL Server** and **PostgreSQL**.

## How to use?

1. Add the [`Toolbelt.EntityFrameworkCore.DecimalAttribute`](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute/) package to your project.

```shell
> dotnet add package Toolbelt.EntityFrameworkCore.DecimalAttribute
```
### Supported Versions

EF Core version              | Package version
-----------------------------|-------------------------
v.3.1, 5, 6, 7, ...11        | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1    
v.3.0                        | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1, v.3.0  
v.2.0, 2.1, 2.2              | v.1.0.x  

2. Annotate your model with the `[Decimal(precision, scale)]` attribute. It lives in the `Toolbelt.ComponentModel.DataAnnotations.Schema.V5` namespace.

```csharp
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

public class Person
{
    public int Id { get; set; }

    [Decimal(10, 5)] // <- Here!
    public decimal Height { get; set; }
}
```

3. **[Important]** Override the `OnModelCreating()` method of your DbContext class, and call the `BuildDecimalColumnTypeFromAnnotations()` extension method. It lives in the `Toolbelt.ComponentModel.DataAnnotations` namespace.

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

The `BuildDecimalColumnTypeFromAnnotations()` extension method scans the DbContext with .NET reflection, finds the `[Decimal]` attributes, and sets the column types in the model.

As a result, the decimal columns in the database that EF Core creates have the precision and scale that you specified with the `[Decimal]` attribute.

## Appendix

### Upgrade an existing project

Follow these steps to upgrade an existing project from ver.3 or earlier to ver.5 or later of this package.

1. Update this package to ver.5 or later.

```
PM> Update-Package Toolbelt.EntityFrameworkCore.DecimalAttribute
```

2. Replace `using Toolbelt.ComponentModel.DataAnnotations.Schema;` with `using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;`.

```csharp
...
// 👇 Remove this line...
// using Toolbelt.ComponentModel.DataAnnotations.Schema;

// 👇 Insert this line, instead.
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
...
```

### If you want to use only "DecimalAttribute" without any dependencies...

If you want to use only the "DecimalAttribute" class without any dependencies, use the [Toolbelt.EntityFrameworkCore.DecimalAttribute.Attribute](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute.Attribute) NuGet package.

## Release Notes

- [Toolbelt.EntityFrameworkCore.DecimalAttribute - Release Notes](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/blob/master/EFCore.DecimalAttribute/RELEASE-NOTES.txt)
- [Toolbelt.EntityFrameworkCore.DecimalAttribute.Attribute - Release Notes](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/blob/master/EFCore.DecimalAttribute.Attribute/RELEASE-NOTES.txt)

## License

[MIT License](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/blob/master/LICENSE)

