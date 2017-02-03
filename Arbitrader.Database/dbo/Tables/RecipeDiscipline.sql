CREATE TABLE [dbo].[RecipeDiscipline]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [disciplineID] INT NULL, 
    CONSTRAINT [FK_RecipeDiscipline_Discipline] FOREIGN KEY ([disciplineID]) REFERENCES [dbo].[Discipline]([ID])
)
