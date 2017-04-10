CREATE TABLE [dbo].[Ingredients]
(
	[pk] INT NOT NULL PRIMARY KEY, 
	[id] INT NOT NULL,
    [itemPK] INT NOT NULL, 
	[recipePK] INT NOT NULL,
	[loadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_Ingredients_Recipe] FOREIGN KEY ([recipePK]) REFERENCES [dbo].[Recipe]([pk]),
	CONSTRAINT [FK_Ingredients_Item] FOREIGN KEY ([itemPK]) REFERENCES [dbo].[Item]([pk])
)
