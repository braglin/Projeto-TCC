CREATE TABLE [dbo].[estoque] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [codigo]     VARCHAR (50)  NULL,
    [descricao]  VARCHAR (200) NULL,
    [fornecedor] VARCHAR (50)  NULL,
    [estoque]    FLOAT (53)    NULL,
    [locacao]    VARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

INSERT INTO [dbo].[estoque] ([id], [codigo], [descricao], [fornecedor], [estoque], [locacao]) VALUES (1, N'SGPA0001', N'GUIA LINEAR ALTA CARGA, HIWIN, HGR20, BARRA 2M', N'HIWIN', 4, N'A1');
INSERT INTO [dbo].[estoque] ([id], [codigo], [descricao], [fornecedor], [estoque], [locacao]) VALUES (2, N'SGPA0002', N'PERFIL DE ALUMINIO 30X60MM EXTRUSADO, BARRA 6M', N'ALPERFIL', 100, N'B1');
INSERT INTO [dbo].[estoque] ([id], [codigo], [descricao], [fornecedor], [estoque], [locacao]) VALUES (4, N'SGPA0003', N'SERVO MOTOR FLANGE DE 60MM', N'EZISERVO', 3, N'C1');
INSERT INTO [dbo].[estoque] ([id], [codigo], [descricao], [fornecedor], [estoque], [locacao]) VALUES (5, N'SGPA0004', N'POLIA DENTADA S5M, DIAMETRO DE 20MM', N'MISUMI', 10, N'C2');
