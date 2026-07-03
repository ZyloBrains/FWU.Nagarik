IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260414172035_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentMarks] (
        [Id] int NOT NULL IDENTITY,
        [RegdNo] nvarchar(50) NOT NULL,
        [SubjectName] nvarchar(200) NOT NULL,
        [SubjectCode] nvarchar(50) NOT NULL,
        [Marks] decimal(5,2) NULL,
        [Grade] nvarchar(10) NULL,
        [Semester] nvarchar(50) NULL,
        [AcademicYear] nvarchar(50) NULL,
        CONSTRAINT [PK_StudentMarks] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260414172035_InitialCreate'
)
BEGIN
    CREATE TABLE [Students] (
        [RegdNo] nvarchar(50) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NOT NULL,
        [DobAD] nvarchar(20) NOT NULL,
        [ProgramName] nvarchar(200) NOT NULL,
        [IntakeYear] nvarchar(50) NOT NULL,
        [StudentStatus] nvarchar(20) NOT NULL,
        [Level] nvarchar(50) NULL,
        [School] nvarchar(100) NULL,
        [CgpaScore] decimal(5,2) NULL,
        [GraduateYear] nvarchar(10) NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([RegdNo])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260414172035_InitialCreate'
)
BEGIN
    CREATE TABLE [VerificationLogs] (
        [Id] int NOT NULL IDENTITY,
        [RegdNo] nvarchar(50) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [MiddleName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [DobAD] nvarchar(20) NOT NULL,
        [ProgramName] nvarchar(200) NOT NULL,
        [IntakeYear] nvarchar(50) NOT NULL,
        [StudentStatus] nvarchar(20) NOT NULL,
        [Level] nvarchar(50) NULL,
        [School] nvarchar(100) NULL,
        [CgpaScore] float NULL,
        [GraduateYear] nvarchar(10) NULL,
        [VerifiedAt] datetime2 NOT NULL,
        [VerificationStatus] nvarchar(20) NOT NULL,
        [ErrorMessage] nvarchar(500) NULL,
        CONSTRAINT [PK_VerificationLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260414172035_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentMarks_RegdNo] ON [StudentMarks] ([RegdNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260414172035_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_VerificationLogs_RegdNo] ON [VerificationLogs] ([RegdNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260414172035_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_VerificationLogs_VerifiedAt] ON [VerificationLogs] ([VerifiedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260414172035_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260414172035_InitialCreate', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [ApiKeys] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Key] nvarchar(64) NOT NULL,
        [Description] nvarchar(200) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NULL,
        [Organization] nvarchar(50) NULL,
        CONSTRAINT [PK_ApiKeys] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NULL,
        [Designation] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [ValidFrom] datetime2 NULL,
        [ValidTo] datetime2 NULL,
        [Remarks] nvarchar(max) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [UserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE TABLE [UserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ApiKeys_Key] ON [ApiKeys] ([Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416150900_AuthTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416150900_AuthTables', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417170332_AddStudentPK'
)
BEGIN
    ALTER TABLE [Students] DROP CONSTRAINT [PK_Students];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417170332_AddStudentPK'
)
BEGIN
    ALTER TABLE [Students] ADD [Id] int NOT NULL IDENTITY;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417170332_AddStudentPK'
)
BEGIN
    ALTER TABLE [Students] ADD CONSTRAINT [PK_Students] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417170332_AddStudentPK'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260417170332_AddStudentPK', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260418060338_StudentRequestModel'
)
BEGIN
    CREATE TABLE [StudentRequests] (
        [Id] int NOT NULL IDENTITY,
        [DocumentType] int NOT NULL,
        [RequestedDate] datetime2 NOT NULL,
        [Requestedby] nvarchar(max) NOT NULL,
        [StudentAdmissionId] int NOT NULL,
        CONSTRAINT [PK_StudentRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentRequests_Students_StudentAdmissionId] FOREIGN KEY ([StudentAdmissionId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260418060338_StudentRequestModel'
)
BEGIN
    CREATE INDEX [IX_StudentRequests_StudentAdmissionId] ON [StudentRequests] ([StudentAdmissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260418060338_StudentRequestModel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260418060338_StudentRequestModel', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    DROP TABLE [StudentMarks];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    ALTER TABLE [Students] ADD [CampusLocation] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    ALTER TABLE [Students] ADD [CampusName] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    ALTER TABLE [Students] ADD [CourseDuration] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    ALTER TABLE [Students] ADD [Faculty] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [Timestamp] datetime2 NOT NULL,
        [Action] nvarchar(50) NOT NULL,
        [EntityType] nvarchar(50) NOT NULL,
        [EntityId] nvarchar(100) NULL,
        [ClientKeyId] nvarchar(50) NULL,
        [ClientName] nvarchar(100) NULL,
        [ClientOrg] nvarchar(100) NULL,
        [ClientIp] nvarchar(45) NULL,
        [UserAgent] nvarchar(500) NULL,
        [RequestMethod] nvarchar(10) NOT NULL,
        [RequestPath] nvarchar(200) NOT NULL,
        [ResponseCode] int NOT NULL,
        [IsSuccess] bit NOT NULL,
        [Details] nvarchar(max) NULL,
        [ErrorMessage] nvarchar(500) NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE TABLE [Institutions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [OfficeName] nvarchar(200) NOT NULL,
        [Location] nvarchar(200) NOT NULL,
        [LogoPath] nvarchar(200) NULL,
        [DocumentTitle] nvarchar(20) NOT NULL,
        [CurrentSerialNo] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Institutions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE TABLE [SyncRecords] (
        [Id] int NOT NULL IDENTITY,
        [EntityName] nvarchar(450) NOT NULL,
        [LastSyncTime] datetime2 NOT NULL,
        [TotalRecordsSynced] int NOT NULL,
        [LoadedBy] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_SyncRecords] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE TABLE [Transcripts] (
        [Id] int NOT NULL IDENTITY,
        [RegdNo] nvarchar(50) NOT NULL,
        [IssueSerialNo] int NOT NULL,
        [IssueDate] datetime2 NOT NULL,
        [IsPrinted] bit NOT NULL,
        [InstitutionId] int NULL,
        CONSTRAINT [PK_Transcripts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Transcripts_Institutions_InstitutionId] FOREIGN KEY ([InstitutionId]) REFERENCES [Institutions] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE TABLE [Semesters] (
        [Id] int NOT NULL IDENTITY,
        [TranscriptId] int NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [SemesterNumber] int NOT NULL,
        [AcademicYear] nvarchar(50) NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_Semesters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Semesters_Transcripts_TranscriptId] FOREIGN KEY ([TranscriptId]) REFERENCES [Transcripts] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE TABLE [Subjects] (
        [Id] int NOT NULL IDENTITY,
        [SemesterId] int NOT NULL,
        [SubjectName] nvarchar(200) NOT NULL,
        [SubjectCode] nvarchar(50) NOT NULL,
        [CreditHours] decimal(5,2) NOT NULL,
        [Grade] nvarchar(10) NOT NULL,
        [GradeValue] decimal(5,2) NOT NULL,
        [GradePoint] decimal(5,2) NOT NULL,
        [CourseType] nvarchar(10) NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Subjects_Semesters_SemesterId] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Action_Timestamp] ON [AuditLogs] ([Action], [Timestamp]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_ClientKeyId_Timestamp] ON [AuditLogs] ([ClientKeyId], [Timestamp]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Timestamp] ON [AuditLogs] ([Timestamp]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_Institutions_IsActive] ON [Institutions] ([IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_Semesters_TranscriptId] ON [Semesters] ([TranscriptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_Subjects_SemesterId] ON [Subjects] ([SemesterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SyncRecords_EntityName] ON [SyncRecords] ([EntityName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_Transcripts_InstitutionId] ON [Transcripts] ([InstitutionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Transcripts_IssueSerialNo] ON [Transcripts] ([IssueSerialNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    CREATE INDEX [IX_Transcripts_RegdNo] ON [Transcripts] ([RegdNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260617031428_AuditLog'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260617031428_AuditLog', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630103839_AddCertificates'
)
BEGIN
    CREATE TABLE [Certificates] (
        [Id] int NOT NULL IDENTITY,
        [RegdNo] nvarchar(50) NOT NULL,
        [ProgramName] nvarchar(200) NOT NULL,
        [CertificateType] nvarchar(50) NOT NULL,
        [BlobName] nvarchar(500) NOT NULL,
        [BlobUrl] nvarchar(500) NOT NULL,
        [OriginalFileName] nvarchar(100) NOT NULL,
        [FileSizeBytes] bigint NOT NULL,
        [UploadedBy] nvarchar(100) NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630103839_AddCertificates'
)
BEGIN
    CREATE INDEX [IX_Certificates_RegdNo] ON [Certificates] ([RegdNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630103839_AddCertificates'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Certificates_RegdNo_ProgramName_CertificateType] ON [Certificates] ([RegdNo], [ProgramName], [CertificateType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630103839_AddCertificates'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260630103839_AddCertificates', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260701155632_AddCSVTranscripts'
)
BEGIN
    DROP TABLE [Certificates];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260701155632_AddCSVTranscripts'
)
BEGIN
    ALTER TABLE [Semesters] ADD [ExamRollNo] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260701155632_AddCSVTranscripts'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260701155632_AddCSVTranscripts', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] DROP CONSTRAINT [FK_Transcripts_Institutions_InstitutionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    DROP TABLE [Institutions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    DROP TABLE [Subjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    DROP TABLE [Semesters];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    DROP INDEX [IX_Transcripts_InstitutionId] ON [Transcripts];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transcripts]') AND [c].[name] = N'InstitutionId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Transcripts] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Transcripts] DROP COLUMN [InstitutionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [AcademicYearName] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [CGPA] decimal(5,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [CollegeName] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [CourseType] nvarchar(10) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [CreditHours] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [ExamRollNo] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [FacultyName] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [Grade] nvarchar(10) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [GradePoint] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [GradeValue] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [Part] nvarchar(10) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [ProgramName] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [SemesterName] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [SemesterNumber] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [SortOrder] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [StudentName] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [SubjectCode] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [SubjectName] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    ALTER TABLE [Transcripts] ADD [Year] nvarchar(10) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702012941_FlattenTranscriptTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702012941_FlattenTranscriptTables', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702015716_UpdateTranscriptUniqueIndex'
)
BEGIN
    DROP INDEX [IX_Transcripts_IssueSerialNo] ON [Transcripts];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702015716_UpdateTranscriptUniqueIndex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Transcripts_RegdNo_IssueSerialNo_SubjectCode] ON [Transcripts] ([RegdNo], [IssueSerialNo], [SubjectCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702015716_UpdateTranscriptUniqueIndex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702015716_UpdateTranscriptUniqueIndex', N'10.0.0');
END;

COMMIT;
GO

