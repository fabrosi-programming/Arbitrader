CREATE TABLE [dbo].[Recipe]
(
	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [type] NVARCHAR(50) NULL, 
    [outputItemID] INT NOT NULL, 
    [outputItemCount] INT NULL, 
    [recipeDisciplineID] INT NOT NULL, 
    [minRating] INT NULL, 
    [ingredientsID] INT NOT NULL, 
    CONSTRAINT [FK_Recipe_Item_outputItemID] FOREIGN KEY ([outputItemID]) REFERENCES [dbo].[Item]([ID]), 
    CONSTRAINT [FK_Recipe_RecipeDiscipline] FOREIGN KEY ([recipeDisciplineID]) REFERENCES [dbo].[RecipeDiscipline]([ID]), 
    CONSTRAINT [FK_Recipe_Ingredients] FOREIGN KEY ([ingredientsID]) REFERENCES [dbo].[Ingredients]([ID])
)
