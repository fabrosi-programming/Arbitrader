CREATE TABLE [dbo].[ItemFlag]
(
	[pk] INT NOT NULL PRIMARY KEY, 
    [flagPK] INT NULL, 
	[loadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ItemFlag_Flag] FOREIGN KEY ([flagPK]) REFERENCES [dbo].[Flag]([pk])
)
