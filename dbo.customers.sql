CREATE TABLE [dbo].[customers] (
    [id_user]    INT   IDENTITY (1, 1)        NOT NULL,
    [surname]    NVARCHAR (50) NOT NULL,
    [name]       NVARCHAR (50) NOT NULL,
    [patronymic] NVARCHAR (50) NOT NULL,
    [telefon]    NVARCHAR(50) NOT NULL,
    [e_mail]     NVARCHAR (50) NOT NULL,
    UNIQUE NONCLUSTERED ([e_mail] ASC),
    CONSTRAINT [PK_customers] PRIMARY KEY CLUSTERED ([id_user] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Фамилия', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'customers', @level2type = N'COLUMN', @level2name = N'surname';

