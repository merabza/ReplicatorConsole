using System.IO;
using AppCliTools.LibDataInput;
using DatabaseTools.DbTools;
using Moq;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorShared.Data.Models;
using ReplicatorShared.Data.Steps;
using Xunit;

namespace ReplicatorConsole.Tests.FieldEditors;

public sealed class LocalPathFieldEditorTests
{
    private const string WorkFolder = @"C:\Work";
    private const string ParametersFolder = @"D:\Params";
    private static readonly string ParametersFileName = Path.Combine(ParametersFolder, "Replicator.json");

    private readonly Mock<IParametersManager> _parametersManager = new();
    private readonly ReplicatorParameters _parameters = new();

    public LocalPathFieldEditorTests()
    {
        _parametersManager.SetupGet(m => m.Parameters).Returns(_parameters);
    }

    private LocalPathFieldEditor CreateDatabaseBackupEditor(string? parametersFileName = null)
    {
        return new LocalPathFieldEditor(nameof(DatabaseBackupStep.LocalPath), _parametersManager.Object,
            nameof(DatabaseBackupStep.DatabaseBackupParameters), parametersFileName);
    }

    private LocalPathFieldEditor CreateFilesBackupEditor(string? parametersFileName = null)
    {
        return new LocalPathFieldEditor(nameof(FilesBackupStep.LocalPath), _parametersManager.Object, null,
            parametersFileName);
    }

    // ---------- Files backup (no database backup parameters property) ----------

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_FilesBackupWithWorkFolder_ReturnsFilesBackupsUnderWorkFolder()
    {
        // Arrange
        _parameters.WorkFolder = WorkFolder;
        LocalPathFieldEditor sut = CreateFilesBackupEditor(ParametersFileName);

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(new FilesBackupStep());

        // Assert
        Assert.Equal(Path.Combine(WorkFolder, "FilesBackups"), result);
    }

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_FilesBackupWithoutWorkFolder_UsesParametersFileFolder()
    {
        // Arrange
        LocalPathFieldEditor sut = CreateFilesBackupEditor(ParametersFileName);

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(new FilesBackupStep());

        // Assert
        Assert.Equal(Path.Combine(ParametersFolder, "FilesBackups"), result);
    }

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_FilesBackupWithoutAnyFolder_ReturnsNull()
    {
        // Arrange
        LocalPathFieldEditor sut = CreateFilesBackupEditor();

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(new FilesBackupStep());

        // Assert
        Assert.Null(result);
    }

    // ---------- Database backup ----------

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_DatabaseBackupParametersNull_ReturnsNull()
    {
        // Arrange
        _parameters.WorkFolder = WorkFolder;
        LocalPathFieldEditor sut = CreateDatabaseBackupEditor();
        var step = new DatabaseBackupStep { DatabaseBackupParameters = null };

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(step);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_BackupTypeNotSet_UsesDefaultFullBackupFolder()
    {
        // Arrange
        _parameters.WorkFolder = WorkFolder;
        LocalPathFieldEditor sut = CreateDatabaseBackupEditor();
        var step = new DatabaseBackupStep { DatabaseBackupParameters = new DatabaseParameters() };

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(step);

        // Assert
        Assert.Equal(Path.Combine(WorkFolder, "DatabaseFullBackups"), result);
    }

    [Theory]
    [InlineData(EBackupType.Full, "DatabaseFullBackups")]
    [InlineData(EBackupType.Diff, "DatabaseDiffBackups")]
    [InlineData(EBackupType.TrLog, "DatabaseTrLogBackups")]
    public void CountWorkFolderCandidateForLocalPath_BackupTypeSet_UsesBackupTypeFolder(EBackupType backupType,
        string expectedFolderName)
    {
        // Arrange
        _parameters.WorkFolder = WorkFolder;
        LocalPathFieldEditor sut = CreateDatabaseBackupEditor();
        var step = new DatabaseBackupStep
        {
            DatabaseBackupParameters = new DatabaseParameters { BackupType = backupType }
        };

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(step);

        // Assert
        Assert.Equal(Path.Combine(WorkFolder, expectedFolderName), result);
    }

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_DatabaseBackupWithoutWorkFolder_UsesParametersFileFolder()
    {
        // Arrange
        LocalPathFieldEditor sut = CreateDatabaseBackupEditor(ParametersFileName);
        var step = new DatabaseBackupStep
        {
            DatabaseBackupParameters = new DatabaseParameters { BackupType = EBackupType.TrLog }
        };

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(step);

        // Assert
        Assert.Equal(Path.Combine(ParametersFolder, "DatabaseTrLogBackups"), result);
    }

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_DatabaseBackupWithoutAnyFolder_ReturnsNull()
    {
        // Arrange
        LocalPathFieldEditor sut = CreateDatabaseBackupEditor();
        var step = new DatabaseBackupStep { DatabaseBackupParameters = new DatabaseParameters() };

        // Act
        string? result = sut.CountWorkFolderCandidateForLocalPath(step);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CountWorkFolderCandidateForLocalPath_RecordWithoutDatabaseBackupParameters_ThrowsDataInputException()
    {
        // Arrange
        LocalPathFieldEditor sut = CreateDatabaseBackupEditor();

        // Act & Assert
        Assert.Throws<DataInputException>(() => sut.CountWorkFolderCandidateForLocalPath(new FilesBackupStep()));
    }
}
