BEGIN TRANSACTION;
ALTER TABLE [Students] DROP CONSTRAINT [PK_Students];

ALTER TABLE [Students] ADD [Id] int NOT NULL IDENTITY;

ALTER TABLE [Students] ADD CONSTRAINT [PK_Students] PRIMARY KEY ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260417170332_AddStudentPK', N'10.0.0');

COMMIT;
GO

