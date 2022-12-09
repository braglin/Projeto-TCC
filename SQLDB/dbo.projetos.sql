CREATE TABLE [dbo].[projetos] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [nome]          VARCHAR (50)  NOT NULL,
    [descricao]     VARCHAR (MAX) NULL,
    [historico]     VARCHAR (MAX) NULL,
    [equipe]        VARCHAR (200) NULL,
    [analise]       INT           NULL,
    [planejamento]  INT           NULL,
    [execucao]      INT           NULL,
    [monitoramento] INT           NULL,
    [encerramento]  INT           NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

INSERT INTO [dbo].[projetos] ([id], [nome], [descricao], [historico], [equipe], [analise], [planejamento], [execucao], [monitoramento], [encerramento]) VALUES (1, N'Robo Cartesiano', N'Desenvolvimento de robo cartesiano de 4 eixos para empilhamento de caixas.', N'[28-11-22] Reunião com o departamento de produção para definir escopo.
[01-12-22] Início do desenvolvimento Mecânico.', N'Fulano, Ciclano', 1, 0, 0, 0, 0);