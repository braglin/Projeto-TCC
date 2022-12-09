CREATE TABLE [dbo].[bom] (
    [id]      INT           IDENTITY (1, 1) NOT NULL,
    [projid]     INT           NOT NULL,
    [um]         INT           NULL,
    [dois]       INT           NULL,
    [tres]       INT           NULL,
    [codigo]     VARCHAR (50)  NULL,
    [descricao]  VARCHAR (200) NOT NULL,
    [qtd]        FLOAT (53)    NOT NULL,
    [unidade]    VARCHAR (2)   NOT NULL,
    [observacao] VARCHAR (200) NULL,
    [designer]   VARCHAR (50)  NOT NULL,
    [revisao]    FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (1, 1, 1, 0, 0, N'', N'Eixo Principal', 1, N'', N'', N'Fulano', 0.1);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (2, 1, 0, 1, 0, N'abc123', N'Barra de guia linear em aço', 3, N'm', N'', N'Fulano', 0.1);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (3, 1, 1, 0, 0, N'', N'Esteira de Entrada', 1, N'', N'', N'Fulano', 0.1);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (4, 1, 0, 1, 0, N'', N'Perfil de Alumínio 30x60mm', 1, N'm', N'', N'Fulano', 0.1);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (5, 1, 0, 2, 0, N'', N'Motor com flange de 60mm', 1, N'pc', N'Kit com caixa de redulção de 1:60', N'Fulano', 0.1);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (6, 1, 0, 3, 0, N'', N'Engrenagem com 60 dentes eixo 8mm', 2, N'pc', N'', N'Fulano', 0.1);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (7, 1, 1, 0, 0, N'', N'Eixo Principal', 1, N'', N'', N'Fulano', 0.2);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (8, 1, 0, 1, 0, N'abc123', N'Barra de guia linear em aço', 3, N'm', N'', N'Fulano', 0.2);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (9, 1, 2, 0, 0, N'', N'Esteira de Entrada', 1, N'', N'', N'Fulano', 0.2);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (10, 1, 0, 1, 0, N'abc234', N'Perfil de Alumínio 30x60mm', 1, N'm', N'', N'Fulano', 0.2);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (11, 1, 0, 2, 0, N'abc345', N'Motor com flange de 60mm', 1, N'pc', N'', N'Fulano', 0.2);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (12, 1, 0, 3, 0, N'abc456', N'Engrenagem com 60 dentes eixo 8mm', 2, N'pc', N'', N'Fulano', 0.2);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (13, 1, 1, 0, 0, N'', N'Eixo Principal', 1, N'', N'', N'Fulano', 0.3);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (14, 1, 0, 1, 0, N'abc123', N'Barra de guia linear em aço', 3, N'm', N'', N'Fulano', 0.3);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (15, 1, 2, 0, 0, N'', N'Esteira de Entrada', 1, N'', N'', N'Fulano', 0.3);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (16, 1, 0, 1, 0, N'abc234', N'Perfil de Alumínio 30x60mm', 1, N'm', N'', N'Fulano', 0.3);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (17, 1, 0, 2, 0, N'abc345', N'Motor com flange de 60mm', 1, N'pc', N'Kit com caixa de redulção de 1:40', N'Fulano', 0.3);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (18, 1, 0, 3, 0, N'abc456', N'Engrenagem com 60 dentes eixo 8mm', 2, N'pc', N'', N'Fulano', 0.3);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (19, 1, 1, 0, 0, N'', N'Eixo Principal', 1, N'', N'', N'Fulano', 0.4);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (20, 1, 0, 1, 0, N'SGPA0001', N'GUIA LINEAR ALTA CARGA, HIWIN, HGR20, BARRA 2M', 2, N'm', N'', N'Fulano', 0.4);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (21, 1, 2, 0, 0, N'', N'Esteira de Entrada', 1, N'', N'', N'Fulano', 0.4);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (22, 1, 0, 1, 0, N'SGPA0002', N'PERFIL DE ALUMINIO 30X60MM EXTRUSADO, BARRA 6M', 1, N'm', N'', N'Fulano', 0.4);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (23, 1, 0, 2, 0, N'SGPA0003', N'SERVO MOTOR FLANGE DE 60MM', 1, N'pc', N'Caixa de redução de 1:40', N'Fulano', 0.4);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (24, 1, 0, 3, 0, N'SGPA0004', N'POLIA DENTADA S5M, DIAMETRO DE 20', 2, N'pc', N'', N'Fulano', 0.4);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (25, 1, 1, 0, 0, N'', N'Eixo Principal', 1, N'', N'', N'Fulano', 0.5);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (26, 1, 0, 1, 0, N'SGPA0001', N'GUIA LINEAR ALTA CARGA, HIWIN, HGR20, BARRA 2M', 2, N'm', N'', N'Fulano', 0.5);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (27, 1, 2, 0, 0, N'', N'Esteira de Entrada', 1, N'', N'', N'Fulano', 0.5);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (28, 1, 0, 1, 0, N'SGPA0002', N'PERFIL DE ALUMINIO 30X60MM EXTRUSADO, BARRA 6M', 1, N'm', N'', N'Fulano', 0.5);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (29, 1, 0, 2, 0, N'SGPA0003', N'SERVO MOTOR FLANGE DE 60MM', 1, N'pc', N'Caixa de redução de 1:40', N'Fulano', 0.5);
INSERT INTO [dbo].[bom] ([id], [projid], [um], [dois], [tres], [codigo], [descricao], [qtd], [unidade], [observacao], [designer], [revisao]) VALUES (30, 1, 0, 3, 0, N'SGPA0004', N'POLIA DENTADA S5M, DIAMETRO DE 20MM', 2, N'pc', N'', N'Fulano', 0.5);