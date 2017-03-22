CREATE TABLE [dbo].[RecipeDiscipline]
(
	[pk] INT NOT NULL PRIMARY KEY, 
	[id] INT NOT NULL,
    [disciplinePK] INT NULL, 
    CONSTRAINT [FK_RecipeDiscipline_Discipline] FOREIGN KEY ([disciplinePK]) REFERENCES [dbo].[Discipline]([pk])
)
