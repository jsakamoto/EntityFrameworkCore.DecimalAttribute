# AGENTS.md

This file provides guidance to AI coding agents when working with code in this repository.

## Overview

NuGet library (`Toolbelt.EntityFrameworkCore.DecimalAttribute`) that lets users annotate entity properties with `[Decimal(precision, scale)]` and then call `modelBuilder.BuildDecimalColumnTypeFromAnnotations()` in `OnModelCreating` to set the column type to `decimal(p, s)`.

## Commands

Solution file is `EFCore.DecimalAttribute.slnx`.

```shell
dotnet build
dotnet test --project EFCore.DecimalAttribute.Test                # all target frameworks (net8.0/9.0/10.0)
dotnet test --project EFCore.DecimalAttribute.Test -f net9.0 --filter-method "*<TestName>*"   # single test
```

Tests run against a real SQL Server (not mocked). By default they connect to `(localdb)\mssqllocaldb` with integrated security and create a throwaway database per test. To use another server, set `MSSQL_SERVER`, `MSSQL_USER`, `MSSQL_PWD` (CI uses a `mcr.microsoft.com/mssql/server` Docker container on `localhost,1433`).

Building either library project packs a `.nupkg` into `..\dist\` (`GeneratePackageOnBuild`).

## Architecture

Two packages, two projects:

- `EFCore.DecimalAttribute.Attribute` (netstandard2.0, no EF dependency): defines the attribute only.
  - `DecimalAttribute.cs` is the legacy, `[Obsolete]` `Toolbelt.ComponentModel.DataAnnotations.Schema.DecimalAttribute`.
  - `V5/DecimalAttribute.cs` is the current public attribute in the `...Schema.V5` namespace. It derives from the legacy one, so the builder only has to handle the legacy base type and picks up both.
- `EFCore.DecimalAttribute` (netstandard2.0/2.1): `AttributedDecimalBuilderExtension` is the whole implementation. It delegates reflection scanning of the DbContext (including owned entity types) to the external `Toolbelt.EntityFrameworkCore.AnnotationBasedModelBuilder` package, and its callback sets `HasColumnType($"decimal({p}, {s})")`. It references the Attribute project.

Keep the legacy attribute type and the V5 inheritance relationship intact. The `V5` namespace exists for backward compatibility with older package versions (see the "Upgrade an existing project" section in `README.md`).

`global.json` switches `dotnet test` to the Microsoft.Testing.Platform runner (needs .NET 10 SDK or later), so use `--project` instead of a positional path and no VSTest options (`-l`, `--logger`).

The test project (xUnit v3 on Microsoft.Testing.Platform, plain `Assert`; models in `EFCore.DecimalAttribute.Test/Models`) multi-targets net8.0/9.0/10.0, each pinned to the matching EF Core version via conditional `PackageReference` groups. When bumping EF Core, update all three groups.

## Releasing

Package release notes come from each project's `RELEASE-NOTES.txt` (read by an MSBuild target at pack time, which extracts the topmost `v.x.y.z` section). Add new entries at the top in that format. The `Version` in both csproj files is kept in sync, and the main package pins `AnnotationBasedModelBuilder` to the same version.
