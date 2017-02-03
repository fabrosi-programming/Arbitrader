CREATE TABLE [dbo].[Item]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [name] NVARCHAR(255) NOT NULL, 
    [type] NVARCHAR(50) NULL, 
    [level] INT NOT NULL, 
    [rarity] NVARCHAR(50) NULL, 
    [vendor_value] INT NULL, 
    [itemFlagID] INT NULL, 
    [icon] NVARCHAR(1023) NULL, 
    CONSTRAINT [FK_Item_ItemFlag] FOREIGN KEY ([itemFlagID]) REFERENCES [dbo].[ItemFlag]([ID])
)
