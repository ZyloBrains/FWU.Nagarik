BEGIN TRANSACTION;
DROP TABLE [StudentMarks];

ALTER TABLE [Students] ADD [CampusLocation] nvarchar(200) NOT NULL DEFAULT N'';

ALTER TABLE [Students] ADD [CampusName] nvarchar(200) NOT NULL DEFAULT N'';

ALTER TABLE [Students] ADD [CourseDuration] nvarchar(50) NOT NULL DEFAULT N'';

ALTER TABLE [Students] ADD [Faculty] nvarchar(50) NOT NULL DEFAULT N'';

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

CREATE TABLE [SyncRecords] (
    [Id] int NOT NULL IDENTITY,
    [EntityName] nvarchar(450) NOT NULL,
    [LastSyncTime] datetime2 NOT NULL,
    [TotalRecordsSynced] int NOT NULL,
    [LoadedBy] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_SyncRecords] PRIMARY KEY ([Id])
);

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

CREATE INDEX [IX_AuditLogs_Action_Timestamp] ON [AuditLogs] ([Action], [Timestamp]);

CREATE INDEX [IX_AuditLogs_ClientKeyId_Timestamp] ON [AuditLogs] ([ClientKeyId], [Timestamp]);

CREATE INDEX [IX_AuditLogs_Timestamp] ON [AuditLogs] ([Timestamp]);

CREATE INDEX [IX_Institutions_IsActive] ON [Institutions] ([IsActive]);

CREATE INDEX [IX_Semesters_TranscriptId] ON [Semesters] ([TranscriptId]);

CREATE INDEX [IX_Subjects_SemesterId] ON [Subjects] ([SemesterId]);

CREATE UNIQUE INDEX [IX_SyncRecords_EntityName] ON [SyncRecords] ([EntityName]);

CREATE INDEX [IX_Transcripts_InstitutionId] ON [Transcripts] ([InstitutionId]);

CREATE UNIQUE INDEX [IX_Transcripts_IssueSerialNo] ON [Transcripts] ([IssueSerialNo]);

CREATE INDEX [IX_Transcripts_RegdNo] ON [Transcripts] ([RegdNo]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260617031428_AuditLog', N'10.0.0');

COMMIT;
GO

