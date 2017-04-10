CREATE TABLE [dbo].[Recipe]
(
	[pk] INT NOT NULL PRIMARY KEY,
	[id] INT NOT NULL,
    [type] NVARCHAR(50) NULL, 
    [outputItemPK] INT NOT NULL,
	[output_item_id] INT NOT NULL,
	[output_item_count] INT NULL,
    [min_rating] INT NULL, 
	[loadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_Recipe_Item_outputItemPK] FOREIGN KEY ([outputItemPK]) REFERENCES [dbo].[Item]([pk]), 
)
