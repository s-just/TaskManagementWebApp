CREATE TABLE [dbo].[AppUser] (
    [UserId]        INT            IDENTITY (1, 1) NOT NULL,
    [Username]      NVARCHAR (50)  NOT NULL,
    [Email]         NVARCHAR (100) NOT NULL,
    [PasswordHash]  NVARCHAR (256) NOT NULL,
    [DateCreated]   DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [LastLoginDate] DATETIME2 (7)  NULL,
    [IsAdmin] BIT NOT NULL, 
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);

