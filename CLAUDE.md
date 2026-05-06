# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository layout & required sibling repos

`ReplicatorConsole.slnx` references projects from **eight sibling repositories** that must be cloned next to this one (i.e. all checked out under the same parent folder). The solution will not load if any are missing. From the `README.md`:

```
mkdir ReplicatorConsole && cd ReplicatorConsole
git clone git@github.com:merabza/ReplicatorConsole.git ReplicatorConsole
git clone git@github.com:merabza/AppCliTools.git AppCliTools
git clone git@github.com:merabza/ConnectionTools.git ConnectionTools
git clone git@github.com:merabza/SystemTools.git SystemTools
git clone git@github.com:merabza/WebAgentContracts.git WebAgentContracts
git clone git@github.com:merabza/DatabaseTools.git DatabaseTools
git clone git@github.com:merabza/ParametersManagement.git ParametersManagement
git clone git@github.com:merabza/ToolsManagement.git ToolsManagement
git clone git@github.com:merabza/ReplicatorShared.git ReplicatorShared.Data
```

This repo only contains the `ReplicatorConsole/ReplicatorConsole.csproj` executable; everything else (menu framework, CRUD primitives, parameter models, DB/file/connection tools) lives in those siblings and is referenced via relative `..\..\<Repo>\<Project>` `ProjectReference`s. When you change a public type used here, expect to also edit one of the sibling repos.

## Build & run

- Target framework: **net10.0** (set in `Directory.Build.props`). To target net8/net9 instead, uncomment the relevant `TargetFramework` line in `Directory.Build.props` and adjust `Directory.Packages.props` accordingly.
- Solution uses the new XML solution format (`.slnx`). Build with `dotnet build ReplicatorConsole.slnx` (or open in VS / Rider).
- Run: `dotnet run --project ReplicatorConsole/ReplicatorConsole.csproj -- --use "<path-to-parameters.json>"`. The `--use <file>` switch is required — `Program.cs` exits with code 1/2 otherwise. `Properties/launchSettings.json` shows the local dev path that's used in the IDE.
- Centralized package versions live in `Directory.Packages.props` (`ManagePackageVersionsCentrally=true`); add `PackageVersion` entries there, not in individual `csproj`s.
- Strict analysis is enforced via `Directory.Build.props`: `TreatWarningsAsErrors=true`, `AnalysisMode=All`, `EnforceCodeStyleInBuild=true`, plus `SonarAnalyzer.CSharp`. Code-style rules from `.editorconfig` are also build-breaking (file-scoped namespaces, no `var` for built-ins, `using` outside namespace, etc.). A clean local build is required before commit.

## Purpose & data model

ReplicatorConsole edits a JSON parameters file (`ReplicatorParameters`, defined in `ReplicatorShared.Data.Models`) that a separate **Replicator** service program later executes as scheduled jobs. The console doesn't run the jobs in production — it's a parameters editor with one-shot "Run this step now" affordances for testing.

The parameters file holds dictionaries of:

- **Schedules** (`JobSchedules`) — when jobs run
- **Steps** — what jobs do. Each step type (`DatabaseBackupStep`, `MultiDatabaseProcessStep`, `RunProgramStep`, `ExecuteSqlCommandStep`, `FilesBackupStep`, `FilesSyncStep`, `FilesMoveStep`, `UnZipOnPlaceStep`) extends `JobStep` and lives in `ReplicatorShared.Data.Steps`. They are stored in matching dictionaries on `ReplicatorParameters` (e.g. `DatabaseBackupSteps`).
- **Step↔schedule binding** (`JobsBySchedules`) — which steps belong to which schedule, with an ordering field.
- **Shared resources** — `DatabaseServerConnections`, `FileStorages`, `Archivers`, `SmartSchemas`, `ExcludeSets`, `ReplacePairsSets`, `ApiClients`. `ReplicatorParameters` implements the corresponding `IParametersWith…` marker interfaces from `ParametersManagement.LibFileParameters.Interfaces`, which is what lets the generic field editors discover them.

`ReplicatorParameters.GetSteps()` flattens all step dictionaries into one keyed-by-name map; `CheckBeforeSave()` prunes orphaned `JobsBySchedules` entries before the file is written.

## Architectural patterns

The whole UI is a console menu loop driven by **strategy + cruder + field-editor** patterns from `AppCliTools`. Understanding these three is essential.

### Bootstrap (`Program.cs` → `ReplicatorConsoleServices.AddServices`)

`Program.cs` is top-level statements: parse args via `ArgumentsParser<ReplicatorParameters>`, build a `ServiceCollection`, then run `CliAppLoop`. DI is wired in `DependencyInjection/ReplicatorConsoleServices.cs`. Notable bindings:

- `IMenuBuilder` → `ReplicatorConsoleMenuBuilder` (this repo's main-menu factory)
- `IParametersManager` → `ParametersManager` (loads/saves the JSON file)
- All `IMenuCommandFactoryStrategy` implementations are auto-registered via `AddTransientAllStrategies<IMenuCommandFactoryStrategy>(typeof(ParametersEditorListCliMenuCommandFactoryStrategy).Assembly)` — so adding a new top-level menu entry is a two-step change: write the strategy class, then list its name in `Menu/MenuData.MainMenuCommandFactoryStrategyNames`.

### Menu strategies (`Menu/<Feature>/…CliMenuCommandFactoryStrategy.cs`)

Each top-level menu item is one folder under `Menu/` containing a `…CliMenuCommandFactoryStrategy` (resolves dependencies, creates the `CliMenuCommand`) plus any cruders/editors specific to that feature. `ReplicatorConsoleMenuBuilder.BuildMainMenu()` calls `CliMenuSetFactory.CreateMenuSet("Main Menu", MenuData.MainMenuCommandFactoryStrategyNames, _serviceProvider, true)` which looks up each name from the DI container and invokes its `CreateMenuCommand()`.

### `StepCruder<TStep>` hierarchy (`StepCruders/`)

Each step type has a cruder extending `StepCruder<TStep> : ParCruder<TStep>` that:

1. Adds the step-type-specific `FieldEditors` (e.g. `DatabaseBackupStepCruder` adds DB connection, smart schema, archiver, etc. fields)
2. Inherits the common `JobStep` editors (`ProcLineId`, `DelayMinutesBeforeStep/After`, `HoleStartTime/EndTime`, `PeriodType`, `FreqInterval`, `StartAt`, `Enabled`) added by the base
3. Overrides `FillDetailsSubMenu` to inject "Run this step now" + per-schedule toggle commands
4. Note the `tempFieldEditors` shuffle in subclasses (e.g. `DatabaseBackupStepCruder.cs:26-63`): the subclass *clears* the inherited list, adds its own fields first, then re-appends the base ones. Field order in the cruder = field order in the UI.

### `ReplicatorParametersEditor` (`Menu/ReplicatorParametersEdit/`)

Edits the top-level scalar/dictionary fields on `ReplicatorParameters` (log folder, work folder, file extensions, plus `DictionaryFieldEditor<TCruder, TItem>` instances for each shared-resource dictionary). When you add a new cross-cutting parameter on `ReplicatorParameters`, register a `FieldEditor` for it here.

### `StandardJobsSchemaGenerator` (`Generators/`)

Triggered by the "Generate Standard Database Jobs" menu command. Connects to a chosen DB server, then idempotently inserts a canonical set of schedules (`Daily`, `AtStart`, `Hourly`), smart schemas (`DailyStandard`, `Reduce`, `Hourly`), archivers, file storages, and Full/TrLog backup + maintenance steps. Read this when you need an end-to-end example of how all the pieces fit together.

## Conventions specific to this repo

- **Code is bilingual**: identifiers and public APIs are English; comments, menu titles shown to the user, and most inline notes are **Georgian**. Don't translate Georgian comments unless asked — they are domain documentation, not noise.
- **End-of-line is CRLF** and final newlines are required (`.editorconfig`). Don't switch a file to LF.
- **`Program Old.cs`, `ReplicatorCliAppLoop.cs`, `ReplicatorServicesCreator.cs`** in the project root are entirely commented-out reference code kept around for the migration to the strategy-based menu builder. Don't delete unless you're sure; do not "uncomment to fix" — the live code paths replaced them.
- **Suppressed warning**: `NU1608` (version conflict) is globally suppressed in `Directory.Build.props` because of cross-repo package alignment.
