# DecimalAttribute for EntityFramework Core  
[![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.EntityFrameworkCore.DecimalAttribute.svg)](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute/) [![unit tests](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/actions/workflows/unit-tests.yml/badge.svg?branch=master&event=push)](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/actions/workflows/unit-tests.yml)

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

The version of EF Core | Version of this package
----------------|-------------------------
v.8.0           | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1  
v.7.0           | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1  
v.6.0           | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1  
v.5.0           | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1  
v.3.1           | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1    
v.3.0           | **v.5.0.1 or later (recommended)**, v.5.0, v.3.1, v.3.0  
v.2.0, 2.1, 2.2 | v.1.0.x  

2. Annotate your model with `[Decimal(precision, scale)]` attribute that lives in `Toolbelt.ComponentModel.DataAnnotations.Schema.V5` namespace.

```csharp
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

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

### Upgrade an existing project

To upgrade an existing project that uses ver.3 or before to use ver.5 or later of this package:
1. Please confirm that the version of this package you use is ver.5 or later.

```
PM> Update-Package Toolbelt.EntityFrameworkCore.DecimalAttribute
```

2. Remove `using Toolbelt.ComponentModel.DataAnnotations.Schema;`, and insert `using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;` instead.

```csharp
...
// 👇 Remove this line...
// using Toolbelt.ComponentModel.DataAnnotations.Schema;

// 👇 Insert this line, instead.
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
...
```

### If you want to use only "DecimalAttribute" without any dependencies...

If you want to use only "DecimalAttribute" class without any dependencies, you can use [Toolbelt.EntityFrameworkCore.DecimalAttribute.Attribute](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.DecimalAttribute.Attribute) NuGet package.

## Release Notes

- [Toolbelt.EntityFrameworkCore.DecimalAttribute - Release Notes](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/blob/master/EFCore.DecimalAttribute/RELEASE-NOTES.txt)
- [Toolbelt.EntityFrameworkCore.DecimalAttribute.Attibute - Release Notes](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/blob/master/EFCore.DecimalAttribute.Attribute/RELEASE-NOTES.txt)

## License

[MIT License](https://github.com/jsakamoto/EntityFrameworkCore.DecimalAttribute/blob/master/LICENSE)

