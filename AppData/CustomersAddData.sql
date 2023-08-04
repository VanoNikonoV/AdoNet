CREATE TABLE [dbo].[customers] (
    [id_user]    INT   IDENTITY (1, 1)        NOT NULL,
    [surname]    NVARCHAR (50) NOT NULL,
    [name]       NVARCHAR (50) NOT NULL,
    [patronymic] NVARCHAR (50) NOT NULL,
    [telefon]    NVARCHAR(11) NOT NULL,
    [e_mail]     NVARCHAR (50) NOT NULL,
    CONSTRAINT [UQ_customer_email] UNIQUE NONCLUSTERED ([e_mail] ASC),
    CONSTRAINT [PK_customers] PRIMARY KEY CLUSTERED ([id_user] ASC)
);
GO
SET IDENTITY_INSERT [dbo].[customers] ON
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (1, N'Виноградова', N'Анжела', N'Васильевна', N'79020000000', N'1@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (2, N'Лебедева', N'Маргарита', N'Александровна', N'79020000001', N'2@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (4, N'Карпова', N'Ольга', N'Дмитриевна', N'79020000002', N'3@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (5, N'Коновалова', N'Аделина', N'Сергеевна', N'79020000003', N'4@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (7, N'Попова', N'Мария', N'Тимофеевна', N'79020000004', N'5@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (8, N'Коновалова', N'Татьяна', N'Васильевна', N'79020000005', N'6@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (10, N'Кузнецова', N'Мария', N'Тимофеевна', N'79020000006', N'7@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (11, N'Коновалова', N'Крестина', N'Владимировна', N'79020000007', N'8@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (13, N'Карпова', N'Людмила', N'Сергеевна', N'79020000008', N'9@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (14, N'Волкова', N'Агата', N'Владимировна', N'79020000009', N'10@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (15, N'Виноградова', N'Аделина', N'Александровна', N'79020000010', N'11@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (16, N'Пономарёва', N'Маргарита', N'Васильевна', N'79020000011', N'12@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (18, N'Попова', N'Анжела', N'Петровна', N'79020000012', N'13@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (19, N'Петрова', N'Ольга', N'Васильевна', N'79020000013', N'14@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (20, N'Пономарёва', N'Вероника', N'Сергеевна', N'79020000014', N'15@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (21, N'Дьячкова', N'Агата', N'Александровна', N'79020000015', N'16@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (22, N'Зайцева', N'Маргарита', N'Ивановна', N'79020000016', N'17@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (24, N'Цветкова', N'Анжела', N'Тимофеевна', N'79020000017', N'18@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (25, N'Соколова', N'Аманда', N'Владимировна', N'79020000018', N'19@mail.ru')
INSERT INTO [dbo].[customers] ([id_user], [surname], [name], [patronymic], [telefon], [e_mail]) VALUES (26, N'Козлова', N'Татьяна', N'Дмитриевна', N'79020000019', N'20@mail.ru')
SET IDENTITY_INSERT [dbo].[customers] OFF
