CREATE TABLE [dbo].[ItemFlag]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [flagID] INT NULL, 
    CONSTRAINT [FK_ItemFlag_Flag] FOREIGN KEY ([flagID]) REFERENCES [dbo].[Flag]([ID])
)
