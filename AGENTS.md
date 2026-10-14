# AGENTS.md

This file provides guidance to AI coding agents when working with code in this repository.

## Overview

NuGet library (`Toolbelt.EntityFrameworkCore.DecimalAttribute`) that lets users annotate entity properties with `[Decimal(precision, scale)]` and then call `modelBuilder.BuildDecimalColumnTypeFromAnnotations()` in `OnModelCreating` to set the column type to `decimal(p, s)`.

## Commands

Solution file is `EFCore.DecimalAttribute.slnx`.

```shell
dotnet build
cd EFCore.DecimalAttribute.Test
dotnet test                                                # all target frameworks (net8.0/9.0/10.0/11.0)
dotnet test -f net9.0 --filter-method "*<TestName>*"       # single test
```

Tests run against a real SQL Server (not mocked). By default they connect to `(localdb)\mssqllocaldb` with integrated security and create a throwaway database per test. To use another server, set `MSSQL_SERVER`, `MSSQL_USER`, `MSSQL_PWD` (CI uses a `mcr.microsoft.com/mssql/server` Docker container on `localhost,1433`).

Building either library project packs a `.nupkg` into `..\dist\` (`GeneratePackageOnBuild`).

## Architecture

Two packages, two projects:

- `EFCore.DecimalAttribute.Attribute` (netstandard2.0, no EF dependency): defines the attribute only.
  - `DecimalAttribute.cs` is the legacy, `[Obsolete]` `Toolbelt.ComponentModel.DataAnnotations.Schema.DecimalAttribute`.
  - `V5/DecimalAttribute.cs` is the current public attribute in the `...Schema.V5` namespace. It derives from the legacy one, so the builder only has to handle the legacy base type and picks up both.
- `EFCore.DecimalAttribute` (netstandard2.0/2.1, net10.0, net11.0): `AttributedDecimalBuilderExtension` is the whole implementation. It delegates reflection scanning of the DbContext (including owned entity types) to the external `Toolbelt.EntityFrameworkCore.AnnotationBasedModelBuilder` package, and its callback sets `HasColumnType($"decimal({p}, {s})")`. It references the Attribute project.
  - The `net10.0` and `net11.0` targets reference EF Core Relational 10 and 11 directly. Without them, the AnnotationBasedModelBuilder package (netstandard assets only) would pull in EF Core 3.x for those targets.

Keep the legacy attribute type and the V5 inheritance relationship intact. The `V5` namespace exists for backward compatibility with older package versions (see the "Upgrade an existing project" section in `README.md`).

`global.json` switches `dotnet test` to the Microsoft.Testing.Platform runner (needs .NET 10 SDK or later), so do not use VSTest options (`-l`, `--logger`). Run `dotnet test` inside the test project folder. The .NET 11 RC SDK resolves `dotnet test --project <folder>` against the wrong path and fails.

The test project (xUnit v3 on Microsoft.Testing.Platform, assertions via ChainingAssertion's `.Is(...)`; models in `EFCore.DecimalAttribute.Test/Models`) multi-targets net8.0/9.0/10.0/11.0, each pinned to the matching EF Core version via conditional `PackageReference` groups. When bumping EF Core, update all four groups.

.NET 11 is still in RC, so every `net11.0` package reference (library and tests) points to `11.0.0-rc.2.26504.105`. When a new RC or the GA ships, update all of them together.

## Releasing

Package release notes come from each project's `RELEASE-NOTES.txt` (read by an MSBuild target at pack time, which extracts the topmost `v.x.y.z` section). Add new entries at the top in that format. The `Version` in both csproj files is kept in sync, and the main package pins `AnnotationBasedModelBuilder` to the same version.

While the `net11.0` target depends on an EF Core 11 RC, packing warns NU5104 (a stable package with a prerelease dependency). Move to the EF Core 11 GA version before you release.
