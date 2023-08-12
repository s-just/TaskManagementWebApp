-- User Table
CREATE TABLE [dbo].[AppUser]
(
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastLoginDate DATETIME2 NULL
);

-- Task Table
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
);

-- Notification Table
CREATE TABLE [dbo].[AppNotification]
(
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL REFERENCES [dbo].[AppUser](UserId), -- FK to AppUser
    Message NVARCHAR(MAX) NOT NULL,
    DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0,
    AssociatedTaskId INT NULL REFERENCES [dbo].[AppTask](TaskId) -- FK to Task
);