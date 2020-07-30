USE StorageDatabase;

SELECT rp.Id, pl.ProductName, rp.Price, rp.[StatusDate] 
FROM [dbo].product_name_list as pl 
INNER JOIN [dbo].received_products as rp ON pl.Id = rp.FK_product_name_list;

SELECT rp.Id, pl.ProductName, rp.Price, rp.[StatusDate]
FROM [dbo].product_name_list as pl
INNER JOIN [dbo].storage_products as rp ON pl.Id = rp.FK_product_name_list;

SELECT rp.Id, pl.ProductName, rp.Price, rp.StatusDate
FROM [dbo].product_name_list as pl
INNER JOIN [dbo].sold_products as rp ON pl.Id = rp.FK_product_name_list;