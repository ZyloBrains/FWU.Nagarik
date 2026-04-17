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

CREATE INDEX [IX_StudentMarks_RegdNo] ON [StudentMarks] ([RegdNo]);

CREATE INDEX [IX_VerificationLogs_RegdNo] ON [VerificationLogs] ([RegdNo]);

CREATE INDEX [IX_VerificationLogs_VerifiedAt] ON [VerificationLogs] ([VerifiedAt]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260414172035_InitialCreate', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
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

CREATE TABLE [Roles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

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

CREATE TABLE [RoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_ApiKeys_Key] ON [ApiKeys] ([Key]);

CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);

CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);

CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260416150900_AuthTables', N'10.0.0');

COMMIT;
GO

