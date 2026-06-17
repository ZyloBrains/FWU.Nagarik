CREATE TABLE [dbo].[AuditLogs] (
    [Id]           INT            NOT NULL IDENTITY,
    [Timestamp]    DATETIME2      NOT NULL,
    [Action]       NVARCHAR(50)   NOT NULL,
    [EntityType]   NVARCHAR(50)   NOT NULL,
    [EntityId]     NVARCHAR(100)  NULL,
    [ClientKeyId]  NVARCHAR(50)   NULL,
    [ClientName]   NVARCHAR(100)  NULL,
    [ClientOrg]    NVARCHAR(100)  NULL,
    [ClientIp]     NVARCHAR(45)   NULL,
    [UserAgent]    NVARCHAR(500)  NULL,
    [RequestMethod] NVARCHAR(10)  NOT NULL,
    [RequestPath]  NVARCHAR(200)  NOT NULL,
    [ResponseCode] INT            NOT NULL,
    [IsSuccess]    BIT            NOT NULL,
    [Details]      NVARCHAR(MAX)  NULL,
    [ErrorMessage] NVARCHAR(500)  NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_AuditLogs_Timestamp] ON [dbo].[AuditLogs] ([Timestamp]);
CREATE INDEX [IX_AuditLogs_ClientKeyId_Timestamp] ON [dbo].[AuditLogs] ([ClientKeyId], [Timestamp]);
CREATE INDEX [IX_AuditLogs_Action_Timestamp] ON [dbo].[AuditLogs] ([Action], [Timestamp]);
