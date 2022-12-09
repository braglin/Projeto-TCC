CREATE TABLE [dbo].[users] (
    [id]      INT          NOT NULL,
    [usuario] VARCHAR (50) NOT NULL,
    [senha]   VARCHAR (50) NOT NULL,
    [tipo]    INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

INSERT INTO [dbo].[users] ([id], [usuario], [senha], [tipo]) VALUES (1, N'gerente1', N'1234', 0),
INSERT INTO [dbo].[users] ([id], [usuario], [senha], [tipo]) VALUES (2, N'comprador1', N'2345', 1),
INSERT INTO [dbo].[users] ([id], [usuario], [senha], [tipo]) VALUES (3, N'desenvolvedor1', N'3456', 2),
