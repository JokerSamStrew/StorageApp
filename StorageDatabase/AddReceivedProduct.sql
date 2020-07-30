USE StorageDatabase;

DELETE FROM [dbo].[product_name_list] WHERE ProductName='new product name';

DECLARE @id int
DECLARE @name varchar(255)
SET @name = 'new product name'
IF NOT EXISTS (SELECT 1 FROM [dbo].[product_name_list] WHERE ProductName=@name) 
	INSERT INTO [dbo].[product_name_list] (ProductName) VALUES (@name)
SET @id = (SELECT Id FROM [dbo].[product_name_list] WHERE ProductName=@name)
INSERT INTO [dbo].[received_products] (FK_product_name_list, Price,	StatusDate) VALUES (@id, 999, '2020-10-10') 
SELECT SCOPE_IDENTITY();

