USE StorageDatabase;


--SELECT 0, 'Total', SUM(t.Price), '00-00-2000', ''
--UNION ALL
--SELECT SUM(t.Price) FROM [dbo].product_name_list as pl
--INNER JOIN (SELECT *, 'Received' as current_status FROM [dbo].received_products as rp
--UNION ALL
--SELECT *, 'Storage' as current_status FROM [dbo].storage_products as sp 
--UNION ALL
--SELECT *, 'Sold' as current_status FROM [dbo].sold_products as sdp) as t 
--ON pl.Id = t.FK_product_name_list;

-- WHERE @FROM_DATE <= t.StatusDate AND t.StatusDate <= @TO_DATE

SELECT SUM(t.Price) FROM (SELECT *  FROM [dbo].received_products as rp UNION ALL SELECT * FROM [dbo].storage_products as sp UNION ALL SELECT * FROM [dbo].sold_products as sdp) as t;
