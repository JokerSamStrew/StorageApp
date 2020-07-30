CREATE TABLE [received_products] (
	[Id]					int NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FK_product_name_list]	int FOREIGN KEY REFERENCES product_name_list(Id),
	[Price]					decimal,
	[StatusDate]			Date
);

