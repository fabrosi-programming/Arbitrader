CREATE TABLE [dbo].[RecipeDiscipline]
(
	[pk] INT NOT NULL PRIMARY KEY, 
	[id] INT NOT NULL,
	[recipePK] INT NOT NULL,
    [disciplinePK] INT NOT NULL, 
	CONSTRAINT [FK_RecipeDiscipline_Recipe] FOREIGN KEY ([recipePK]) REFERENCES [dbo].[Recipe]([pk]),
    CONSTRAINT [FK_RecipeDiscipline_Discipline] FOREIGN KEY ([disciplinePK]) REFERENCES [dbo].[Discipline]([pk])
)
