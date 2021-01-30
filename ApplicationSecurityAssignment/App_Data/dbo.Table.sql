CREATE TABLE [dbo].[Table]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [firstname] NCHAR(20) NULL, 
    [lastname] NCHAR(20) NULL, 
    [creditcard] INT NULL, 
    [emailaddress] NCHAR(20) NULL, 
    [passwordhash] NVARCHAR(MAX) NULL, 
    [passwordsalt] NVARCHAR(MAX) NULL, 
    [dob] DATE NULL
)
