USE StorageDatabase;

INSERT INTO [dbo].storage_products 
	([dbo].storage_products.FK_product_name_list, 
	 [dbo].storage_products.Price, 
	 [dbo].storage_products.StatusDate)
SELECT FK_product_name_list as fk, Price, GETDATE()
FROM [dbo].received_products as rp
WHERE rp.Id=1
DELETE FROM [dbo].received_products WHERE [dbo].received_products.Id = 1
SELECT SCOPE_IDENTITY(), StatusDate FROM [dbo].storage_products WHERE Id=SCOPE_IDENTITY();