BEGIN TRANSACTION;
CREATE TABLE [StudentRequests] (
    [Id] int NOT NULL IDENTITY,
    [DocumentType] int NOT NULL,
    [RequestedDate] datetime2 NOT NULL,
    [Requestedby] nvarchar(max) NOT NULL,
    [StudentAdmissionId] int NOT NULL,
    CONSTRAINT [PK_StudentRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentRequests_Students_StudentAdmissionId] FOREIGN KEY ([StudentAdmissionId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_StudentRequests_StudentAdmissionId] ON [StudentRequests] ([StudentAdmissionId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260418060338_StudentRequestModel', N'10.0.0');

COMMIT;
GO

