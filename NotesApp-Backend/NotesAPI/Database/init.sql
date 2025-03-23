CREATE DATABASE NotesApp;
GO

USE NotesApp;
GO

-- Users table
CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username] NVARCHAR(100) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(200) NOT NULL
);

-- Notes table
CREATE TABLE [dbo].[Notes] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] INT,
    [Title] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(MAX),
    [CreatedAt] DATETIME,
    [UpdatedAt] DATETIME
);
