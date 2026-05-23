# ReplicatorConsole

ReplicatorConsole არის **Replicator** სერვისის კონფიგურაციის (JSON) ფაილის რედაქტორი. თვითონ კონსოლი არ ასრულებს ავტომატურ სამუშაოებს — ის რედაქტირებას უწევს პარამეტრებს, რომელთა მიხედვითაც Replicator სერვისი დაგეგმილ დროებში შეასრულებს დავალებებს. ასევე გვაქვს "Run this step now" ფუნქცია ცალკეული ნაბიჯის ერთჯერადი ტესტური გაშვებისთვის.

> **EN:** ReplicatorConsole is a console editor for the parameters file (JSON) used by the **Replicator** service. The console itself does not execute scheduled jobs — it only edits the configuration that Replicator later runs. A "Run this step now" affordance is available for one-shot testing.

## დანიშნულება / Purpose

რედაქტირდება ყოველდღიური ავტომატური სამუშაოების კონფიგურაცია: გრაფიკები, ნაბიჯები, მათი ერთმანეთთან მიბმა და გაზიარებული რესურსები (ბაზის სერვერების კავშირები, ფაილების საცავები, არქივატორები, smart schema-ები, replace-pair-ები, API client-ები).

> **EN:** The editor manages job schedules, individual steps, schedule↔step bindings, and shared resources (DB connections, file storages, archivers, smart schemas, replace-pairs, API clients).

## ნაბიჯების ტიპები / Step Types

| ნაბიჯი / Step | აღწერა / Description |
| --- | --- |
| `DatabaseBackupStep` | მონაცემთა ბაზის ბექაპირება / Database backup |
| `MultiDatabaseProcessStep` | რამდენიმე ბაზის პროფილაქტიკური დამუშავება / Multi-database maintenance |
| `RunProgramStep` | გარე პროგრამის გაშვება / Run external program |
| `ExecuteSqlCommandStep` | SQL ბრძანების/სკრიპტის გაშვება / Execute SQL command or script |
| `FilesBackupStep` | ფაილების ბექაპირება / Files backup |
| `FilesSyncStep` | ფაილების სინქრონიზაცია / Files sync |
| `FilesMoveStep` | ფაილების გადაადგილება / Files move |
| `UnZipOnPlaceStep` | არქივის ადგილზე განარქივება / Unzip in place |

## საჭირო რეპოზიტორიები / Required Repositories

`ReplicatorConsole.slnx` მიუთითებს **8 sibling რეპოზიტორიაზე**, რომლებიც ერთიდაიგივე მშობელი საქაღალდის ქვეშ უნდა იყოს დაკლონირებული. სოლუშენი არ ჩაიტვირთება, თუ ერთიც აკლია.

> **EN:** The solution references 8 sibling repositories that must be cloned under the same parent folder. The solution will not load if any are missing.

```bash
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
cd ..
```

## აწყობა და გაშვება / Build & Run

- **მოთხოვნა / Requirement:** .NET 10 SDK
- **Build:** `dotnet build ReplicatorConsole.slnx`
- **Run:** `dotnet run --project ReplicatorConsole/ReplicatorConsole.csproj -- --use "<path-to-parameters.json>"`

`--use <file>` სავალდებულო პარამეტრია — მის გარეშე პროგრამა ჩერდება შეცდომის კოდით 1 ან 2. პაკეტების ვერსიები ცენტრალიზებულად მართულია `Directory.Packages.props`-ში.

> **EN:** The `--use <file>` switch is required; without it the program exits with code 1 or 2. Package versions are centrally managed in `Directory.Packages.props`.

## არქიტექტურა / Architecture

მთელი UI არის კონსოლის მენიუს ციკლი, რომელიც აგებულია **Strategy + Cruder + FieldEditor** პატერნებზე (AppCliTools-დან). მთავარი მენიუს პუნქტები ჩამოთვლილია [Menu/MenuData.cs](ReplicatorConsole/Menu/MenuData.cs)-ში — ახალი მენიუ-პუნქტის დასამატებლად საჭიროა strategy კლასი + მისი სახელის ჩამატება ამ სიაში.

> **EN:** The UI is a console menu loop built on **Strategy + Cruder + FieldEditor** patterns from AppCliTools. Main menu items are listed in [Menu/MenuData.cs](ReplicatorConsole/Menu/MenuData.cs) — adding a new top-level item requires a strategy class plus an entry in that list.
