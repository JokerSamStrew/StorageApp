CREATE TABLE [product_name_list] (
	[Id]					int NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ProductName]			varchar(255) NOT NULL UNIQUE
);