CREATE TABLE [dbo].[kanban] (
    [id]        INT          IDENTITY (1, 1) NOT NULL,
    [projid]    INT          NOT NULL,
    [descricao] VARCHAR (50) NOT NULL,
    [equipe]    VARCHAR (50) NOT NULL,
    [categoria] INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

INSERT INTO [dbo].[kanban] ([id], [projid], [descricao], [equipe], [categoria]) VALUES (1, 1, N'Reunião inicial do projeto', N'Todos', 2);
INSERT INTO [dbo].[kanban] ([id], [projid], [descricao], [equipe], [categoria]) VALUES (2, 1, N'Desenvolvimento Mecânico', N'Fulano', 1);