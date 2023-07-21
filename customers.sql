CREATE TABLE [dbo].[customers] (
    [id_user]    INT           NOT NULL,
    [surname]    NVARCHAR (50) NOT NULL,
    [name]       NVARCHAR (50) NOT NULL,
    [patronymic] NVARCHAR (50) NOT NULL,
    [telefon]    NVARCHAR (10) NOT NULL,
    [e_mail]     NVARCHAR (50) UNIQUE NOT NULL,
    CONSTRAINT [PK_customers] PRIMARY KEY CLUSTERED ([id_user] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Фамилия', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'customers', @level2type = N'COLUMN', @level2name = N'surname';

