CREATE TABLE [dbo].[AppTask]
(
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DueDate DATETIME2 NULL,
    Status INT NOT NULL, 
    AssignedToUserId INT NULL REFERENCES [dbo].[AppUser](UserId),
    CreatedByUserId INT NOT NULL REFERENCES [dbo].[AppUser](UserId),
    DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME2 NULL
)