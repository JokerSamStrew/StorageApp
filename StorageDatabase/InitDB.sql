USE StorageDatabase;


DELETE FROM [dbo].[received_products];
DELETE FROM [dbo].[sold_products];
DELETE FROM [dbo].[storage_products];
DELETE FROM [dbo].[product_name_list];

DBCC CHECKIDENT ([received_products], RESEED, 0 )
DBCC CHECKIDENT ([sold_products], RESEED, 0 )
DBCC CHECKIDENT ([storage_products], RESEED, 0 )
DBCC CHECKIDENT ([product_name_list], RESEED, 0 )

INSERT INTO [dbo].[product_name_list] (ProductName)
VALUES 
	('Received Product Name 1'),
	('Received Product Name 2'),
	('Received Product Name 3'),
	('Received Product Name 4'),
	('Storage Product Name 1'),
	('Storage Product Name 2'),
	('Storage Product Name 3'),
	('Storage Product Name 4'),
	('Sold Product Name 1'),
	('Sold Product Name 2'),
	('Sold Product Name 3'),
	('Sold Product Name 4');

INSERT INTO [dbo].[received_products] (FK_product_name_list, Price, [StatusDate])
VALUES 
	(1,  100, '2020-01-01'),
	(2,  200, '2020-02-02'),
	(3,  300, '2020-03-03'),
	(4,  400, '2020-04-04');

INSERT INTO [dbo].[storage_products] (FK_product_name_list, Price, [StatusDate])
VALUES 
	(5,  101, '2019-01-01'),
	(6,  201, '2019-02-02'),
	(7,  301, '2019-03-03'),
	(8,  401, '2019-04-04');

INSERT INTO [dbo].[sold_products] (FK_product_name_list, Price, [StatusDate])
VALUES 
	(9,  102, '2018-01-01'),
	(10, 202, '2018-02-02'),
	(11, 302, '2018-03-03'),
	(12, 402, '2018-04-04');