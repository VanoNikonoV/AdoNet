CREATE TABLE [dbo].[register] (
    [id_user]       INT          IDENTITY (1, 1) NOT NULL,
    [login_user]    VARCHAR (50) NOT NULL,
    [password_user] VARCHAR (50) NOT NULL,
    UNIQUE NONCLUSTERED ([login_user] ASC), 
    CONSTRAINT [PK_register] PRIMARY KEY ([id_user])
);

