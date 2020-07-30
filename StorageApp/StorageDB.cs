using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Windows;

namespace StorageApp
{


    static class StorageDB
    {
        public static string ConnectionString
        {
            get
            {
                return _connection_string;
            }

            set
            {
                _connection_string = value;
            }
        }

        private static string _connection_string = @"Database=StorageDatabase;Data Source=DESKTOP-3RC1KGR\SQLEXPRESS;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False";

        private const string _select_product_names_query = @"SELECT [Id], [ProductName] FROM[StorageDatabase].[dbo].[product_name_list]";

        private const string _received_products_query = @"SELECT rp.Id, pl.ProductName, rp.Price, rp.StatusDate FROM [dbo].product_name_list as pl INNER JOIN [dbo].received_products as rp ON pl.Id = rp.FK_product_name_list;";

        private const string _storage_products_query = @"SELECT rp.Id, pl.ProductName, rp.Price, rp.StatusDate FROM [dbo].product_name_list as pl INNER JOIN [dbo].storage_products as rp ON pl.Id = rp.FK_product_name_list;";

        private const string _sold_products_query = @"SELECT [rp].[Id], [pl].[ProductName], [rp].[Price], [rp].[StatusDate] FROM [dbo].[product_name_list] as pl INNER JOIN [dbo].[sold_products] as rp ON [pl].[Id] = [rp].[FK_product_name_list];";

        private const string _add_received_product_query = @"DECLARE @id int IF NOT EXISTS (SELECT 1 FROM [dbo].[product_name_list] WHERE ProductName=@NAME) INSERT INTO [dbo].[product_name_list] (ProductName) VALUES (@NAME) SET @id = (SELECT Id FROM [dbo].[product_name_list] WHERE ProductName=@NAME) INSERT INTO [dbo].[received_products] (FK_product_name_list, Price, StatusDate) VALUES (@id, @PRICE, @STATUSDATE) SELECT SCOPE_IDENTITY()";

        private const string _move_from_received_to_storage_by_id = @"INSERT INTO [dbo].storage_products ([dbo].storage_products.FK_product_name_list, [dbo].storage_products.Price, [dbo].storage_products.StatusDate) SELECT FK_product_name_list as fk, Price, GETDATE() FROM [dbo].received_products as rp WHERE rp.Id = @ID DELETE FROM [dbo].received_products WHERE [dbo].received_products.Id = @ID SELECT SCOPE_IDENTITY(), StatusDate FROM [dbo].storage_products WHERE Id=SCOPE_IDENTITY();";

        private const string _move_from_storage_to_sold_by_id = @"INSERT INTO [dbo].sold_products ([dbo].sold_products.FK_product_name_list, [dbo].sold_products.Price, [dbo].sold_products.StatusDate) SELECT FK_product_name_list as fk, Price, GETDATE() FROM [dbo].storage_products as rp WHERE rp.Id = @ID DELETE FROM [dbo].storage_products WHERE [dbo].storage_products.Id = @ID SELECT SCOPE_IDENTITY(), StatusDate FROM [dbo].sold_products WHERE Id=SCOPE_IDENTITY();";

        private const string _all_records_query = @"SELECT t.Id, pl.ProductName, t.Price, t.StatusDate, current_status FROM [dbo].product_name_list as pl INNER JOIN (SELECT *, 'Received' as current_status FROM [dbo].received_products as rp UNION ALL SELECT *, 'Storage' as current_status FROM [dbo].storage_products as sp UNION ALL SELECT *, 'Sold' as current_status FROM [dbo].sold_products as sdp) as t ON pl.Id = t.FK_product_name_list WHERE @FROM_DATE <= t.StatusDate AND t.StatusDate <= @TO_DATE;";

        private const string _received_products_interval_query = @"SELECT rp.Id, pl.ProductName, rp.Price, rp.StatusDate FROM [dbo].product_name_list as pl INNER JOIN [dbo].received_products as rp ON pl.Id = rp.FK_product_name_list WHERE @FROM_DATE <= rp.StatusDate AND rp.StatusDate <= @TO_DATE;";

        private const string _storage_products_interval_query = @"SELECT rp.Id, pl.ProductName, rp.Price, rp.StatusDate FROM [dbo].product_name_list as pl INNER JOIN [dbo].storage_products as rp ON pl.Id = rp.FK_product_name_list WHERE @FROM_DATE <= rp.StatusDate AND rp.StatusDate <= @TO_DATE;";

        private const string _sold_products_interval_query = @"SELECT [rp].[Id], [pl].[ProductName], [rp].[Price], [rp].[StatusDate] FROM [dbo].[product_name_list] as pl INNER JOIN [dbo].[sold_products] as rp ON [pl].[Id] = [rp].[FK_product_name_list] WHERE @FROM_DATE <= rp.StatusDate AND rp.StatusDate <= @TO_DATE;";

        private const string _sum_all_products_interval_query = @"SELECT SUM(t.Price) FROM (SELECT *  FROM [dbo].received_products as rp UNION ALL SELECT * FROM [dbo].storage_products as sp UNION ALL SELECT * FROM [dbo].sold_products as sdp) as t;";

        private const string _sum_received_products_interval_query = @"SELECT SUM([rp].[Price]) FROM [dbo].[received_products] as rp WHERE @FROM_DATE <= rp.StatusDate AND rp.StatusDate <= @TO_DATE;";

        private const string _sum_storage_products_interval_query = @"SELECT SUM([rp].[Price]) FROM [dbo].[storage_products] as rp WHERE @FROM_DATE <= rp.StatusDate AND rp.StatusDate <= @TO_DATE;";

        private const string _sum_sold_products_interval_query = @"SELECT SUM([rp].[Price]) FROM [dbo].[sold_products] as rp WHERE @FROM_DATE <= rp.StatusDate AND rp.StatusDate <= @TO_DATE;";

        public static decimal CountAllProductSum(DateTime from_date, DateTime to_date)
        {
            return CountProductSum(_sum_all_products_interval_query, from_date, to_date);
        }

        public static decimal CountReceivedProductSum(DateTime from_date, DateTime to_date)
        {
            return CountProductSum(_sum_received_products_interval_query, from_date, to_date);
        }
        public static decimal CountStorageProductSum(DateTime from_date, DateTime to_date)
        {
            return CountProductSum(_sum_storage_products_interval_query, from_date, to_date);
        }
        public static decimal CountSoldProductSum(DateTime from_date, DateTime to_date)
        {
            return CountProductSum(_sum_sold_products_interval_query, from_date, to_date);
        }
        public static List<ProductRecord> QueryAllProductRecords(DateTime from_date, DateTime to_date)
        {
            List<ProductRecord> products_records = new List<ProductRecord>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connection_string))
                {

                    conn.Open();
                    SqlCommand command = new SqlCommand(_all_records_query, conn);
                    command.Parameters.Add("@FROM_DATE", System.Data.SqlDbType.Date);
                    command.Parameters["@FROM_DATE"].Value = from_date;
                    command.Parameters.Add("@TO_DATE", System.Data.SqlDbType.Date);
                    command.Parameters["@TO_DATE"].Value = to_date;

                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        products_records.Add(new ProductRecord()
                        {
                            ProductId = dataReader.GetInt32(0),
                            ProductName = dataReader.GetString(1),
                            Price = dataReader.GetDecimal(2),
                            Date = dataReader.GetDateTime(3),
                            CurrentStatus = dataReader.GetString(4)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return products_records;
        }
        public static List<ProductRecord> QueryReceivedProductRecords(DateTime from_date, DateTime to_date)
        {
            return QueryProductRecords(_received_products_interval_query, from_date, to_date);
        }
        public static List<ProductRecord> QueryStorageProductRecords(DateTime from_date, DateTime to_date)
        {
            return QueryProductRecords(_storage_products_interval_query, from_date, to_date);
        }
        public static List<ProductRecord> QuerySoldProductRecords(DateTime from_date, DateTime to_date)
        {
            return QueryProductRecords(_sold_products_interval_query, from_date, to_date);
        }
        public static List<ProductRecord> QueryReceivedProductRecords()
        {
            return QueryProductRecords(_received_products_query);
        }
        public static List<ProductRecord> QueryStorageProductRecords()
        {
            return QueryProductRecords(_storage_products_query);
        }
        public static List<ProductRecord> QuerySoldProductRecords()
        {
            return QueryProductRecords(_sold_products_query);
        }
        private static List<ProductRecord> QueryProductRecords(string command_string)
        {
            List<ProductRecord> products_records = new List<ProductRecord>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connection_string))
                {

                    conn.Open();
                    SqlCommand command = new SqlCommand(command_string, conn);
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        products_records.Add(new ProductRecord()
                        {
                            ProductId = dataReader.GetInt32(0),
                            ProductName = dataReader.GetString(1),
                            Price = dataReader.GetDecimal(2),
                            Date = dataReader.GetDateTime(3)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return products_records;
        }

        private static List<ProductRecord> QueryProductRecords(string command_string, DateTime from_date, DateTime to_date)
        {
            List<ProductRecord> products_records = new List<ProductRecord>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connection_string))
                {

                    conn.Open();
                    SqlCommand command = new SqlCommand(command_string, conn);
                    command.Parameters.Add("@FROM_DATE", System.Data.SqlDbType.Date);
                    command.Parameters["@FROM_DATE"].Value = from_date;
                    command.Parameters.Add("@TO_DATE", System.Data.SqlDbType.Date);
                    command.Parameters["@TO_DATE"].Value = to_date;

                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        products_records.Add(new ProductRecord()
                        {
                            ProductId = dataReader.GetInt32(0),
                            ProductName = dataReader.GetString(1),
                            Price = dataReader.GetDecimal(2),
                            Date = dataReader.GetDateTime(3)
                        });
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return products_records;
        }

        public static List<string> QueryProductsNames()
        {
            List<string> products_names = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connection_string))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(_select_product_names_query, conn);
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        products_names.Add(dataReader.GetString(1));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return products_names;
        }

        // Changes new_product ProductId
        public static void AddNewProduct(ProductRecord new_product)
        {
            using (SqlConnection conn = new SqlConnection(_connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(_add_received_product_query, conn);
                command.Parameters.Add("@NAME", System.Data.SqlDbType.VarChar);
                command.Parameters["@NAME"].Value = new_product.ProductName;
                command.Parameters.Add("@PRICE", System.Data.SqlDbType.Decimal);
                command.Parameters["@PRICE"].Value = new_product.Price;
                command.Parameters.Add("@STATUSDATE", System.Data.SqlDbType.Date);
                command.Parameters["@STATUSDATE"].Value = new_product.Date;

                SqlDataReader dataReader = command.ExecuteReader();
                dataReader.Read();

                new_product.ProductId = (int)dataReader.GetDecimal(0);
            }
        }

        private static void MoveToTable(ProductRecord product, string query)
        {
            using (SqlConnection conn = new SqlConnection(_connection_string))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = product.ProductId;

                SqlDataReader dataReader = command.ExecuteReader();
                dataReader.Read();

                product.ProductId = (int)dataReader.GetDecimal(0);
                product.Date = dataReader.GetDateTime(1);
            }
        }

        private static decimal CountProductSum(string command_string, DateTime from_date, DateTime to_date)
        {
            decimal sum = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connection_string))
                {

                    conn.Open();
                    SqlCommand command = new SqlCommand(command_string, conn);
                    command.Parameters.Add("@FROM_DATE", System.Data.SqlDbType.Date);
                    command.Parameters["@FROM_DATE"].Value = from_date;
                    command.Parameters.Add("@TO_DATE", System.Data.SqlDbType.Date);
                    command.Parameters["@TO_DATE"].Value = to_date;

                    SqlDataReader dataReader = command.ExecuteReader();

                    dataReader.Read();
                    sum = dataReader.GetDecimal(0);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }

            return sum;
        }

        // Changes received_product ProductId and date
        public static void MoveToStorage(ProductRecord received_product)
        {
            MoveToTable(received_product, _move_from_received_to_storage_by_id);
        }

        // Changes storage_product ProductId and date
        public static void MoveToSold(ProductRecord storage_product)
        {
            MoveToTable(storage_product, _move_from_storage_to_sold_by_id);
        }
    }
}
