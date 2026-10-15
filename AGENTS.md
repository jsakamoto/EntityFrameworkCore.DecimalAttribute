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

Tests run against a real SQL Server (not mocked). The test code starts it in a Docker container with Testcontainers (`SqlServerFixture.cs`, image `mcr.microsoft.com/mssql/server:2025-latest`), so a running Docker engine is the only requirement, locally and in CI. SQL Server test classes join the `SqlServerCollection` xUnit collection to share one container per test run (one per target framework), and each test creates its own throwaway database with `SqlServerFixture.CreateDatabase()`. `IndexAttribute` uses the same setup, so keep the two in sync.

Building either library project packs a `.nupkg` into `..\dist\` (`GeneratePackageOnBuild`).

## Architecture

Two packages, two projects:

- `EFCore.DecimalAttribute.Attribute` (netstandard2.0, no EF dependency): defines the attribute only.
  - `DecimalAttribute.cs` is the legacy, `[Obsolete]` `Toolbelt.ComponentModel.DataAnnotations.Schema.DecimalAttribute`.
  - `V5/DecimalAttribute.cs` is the current public attribute in the `...Schema.V5` namespace. It derives from the legacy one, so the builder only has to handle the legacy base type and picks up both.
- `EFCore.DecimalAttribute` (netstandard2.0/2.1, net10.0, net11.0): `AttributedDecimalBuilderExtension` is the whole implementation. It delegates reflection scanning of the DbContext (including owned entity types) to the external `Toolbelt.EntityFrameworkCore.AnnotationBasedModelBuilder` package, and its callback sets `HasColumnType($"decimal({p}, {s})")`. It references the Attribute project.

Keep the legacy attribute type and the V5 inheritance relationship intact. The `V5` namespace exists for backward compatibility with older package versions (see the "Upgrade an existing project" section in `README.md`).

`global.json` switches `dotnet test` to the Microsoft.Testing.Platform runner (needs .NET 10 SDK or later), so do not use VSTest options (`-l`, `--logger`). Run `dotnet test` inside the test project folder. The .NET 11 RC SDK resolves `dotnet test --project <folder>` against the wrong path and fails.

The test project uses xUnit v3 on Microsoft.Testing.Platform, with assertions via ChainingAssertion's `.Is(...)`. Its models are in `EFCore.DecimalAttribute.Test/Models`.

## Target frameworks and EF Core versions

Follow these three rules.

1. **The library lists each .NET version from `net10.0` on.** `EFCore.DecimalAttribute` targets `netstandard2.0` and `netstandard2.1`, plus `net10.0` and `net11.0` one by one. When a new .NET major version ships, add its TFM. These TFMs are there so that nuget.org shows them as supported. (`EFCore.DecimalAttribute.Attribute` has no EF dependency and stays `netstandard2.0` only.)
2. **The library keeps the oldest EF Core reference on every TFM.** It has no direct EF Core reference. EF Core comes through the AnnotationBasedModelBuilder package, which depends on EF Core Relational 3.0.0 (3.1.0 for `netstandard2.0`). This keeps one package usable with EF Core 3.0 through the latest. Never add a newer EF Core reference, not even for `net10.0` or later. It would force users to upgrade EF Core, for example a .NET 10 app that still uses EF Core 8.
3. **The test project uses the latest EF Core of the same major version as each TFM.** It targets `net8.0` to `net11.0`, and each TFM has a conditional `PackageReference` group (for example, `net10.0` uses the latest EF Core 10.0.x). The other Microsoft packages in the group (`System.Runtime.Caching`, `Microsoft.Extensions.Logging.Debug`) follow the same rule. While a .NET version is still in preview or RC, use its latest prerelease (now `11.0.0-rc.2.26504.105`). When you bump versions, update every group.

## Releasing

Package release notes come from each project's `RELEASE-NOTES.txt` (read by an MSBuild target at pack time, which extracts the topmost `v.x.y.z` section). Add new entries at the top in that format. The `Version` in both csproj files is kept in sync, and the main package pins `AnnotationBasedModelBuilder` to the same version.
