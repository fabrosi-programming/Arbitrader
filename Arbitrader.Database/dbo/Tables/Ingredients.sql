CREATE TABLE [dbo].[Ingredients]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [itemID] INT NOT NULL, 
    CONSTRAINT [FK_Ingredients_Item] FOREIGN KEY ([itemID]) REFERENCES [dbo].[Item]([ID])
)
