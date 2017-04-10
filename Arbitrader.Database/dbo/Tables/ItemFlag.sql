CREATE TABLE [dbo].[ItemFlag]
(
	[pk] INT NOT NULL PRIMARY KEY, 
	[itemPK] INT NULL,
    [flagPK] INT NULL, 
	[loadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
	CONSTRAINT [FK_ItemFlag_Item] FOREIGN KEY ([itemPK]) REFERENCES [dbo].[Item]([pk]),
    CONSTRAINT [FK_ItemFlag_Flag] FOREIGN KEY ([flagPK]) REFERENCES [dbo].[Flag]([pk])
)
