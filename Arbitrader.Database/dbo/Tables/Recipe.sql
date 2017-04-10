CREATE TABLE [dbo].[Recipe]
(
	[pk] INT NOT NULL PRIMARY KEY,
	[id] INT NOT NULL,
    [type] NVARCHAR(50) NULL, 
    [outputItemPK] INT NOT NULL, 
    [outputItemCount] INT NULL, 
    [minRating] INT NULL, 
    [ingredientsPK] INT NOT NULL,
	[loadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_Recipe_Item_outputItemPK] FOREIGN KEY ([outputItemPK]) REFERENCES [dbo].[Item]([pk]), 
    CONSTRAINT [FK_Recipe_Ingredients] FOREIGN KEY ([ingredientsPK]) REFERENCES [dbo].[Ingredients]([pk])
)
