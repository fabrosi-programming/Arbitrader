CREATE TABLE [dbo].[Item]
(
	[pk] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	[id] INT NOT NULL,
    [name] NVARCHAR(255) NOT NULL, 
    [type] NVARCHAR(50) NULL, 
    [level] INT NOT NULL, 
    [rarity] NVARCHAR(50) NULL, 
    [vendor_value] INT NULL, 
    [itemFlagPK] INT NULL, 
    [icon] NVARCHAR(1023) NULL, 
	[loadDate] DATETIME2 NOT NULL DEFAULT GETDATE()
)
