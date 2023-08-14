CREATE TABLE [dbo].[AppNotification]
(
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL REFERENCES [dbo].[AppUser](UserId), -- FK to AppUser
    Message NVARCHAR(MAX) NOT NULL,
    DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0,
    AssociatedTaskId INT NULL REFERENCES [dbo].[AppTask](TaskId) -- FK to Task
)