using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace StorageApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                using (SqlConnection test_conn = new SqlConnection((string)Properties.Settings.Default["ConnectionString"]))
                {
                    test_conn.Open();
                }
                StorageDB.ConnectionString = (string)Properties.Settings.Default["ConnectionString"];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                string connection_string = Interaction.InputBox("Prompt", "Title", (string)Properties.Settings.Default["ConnectionString"]);
                Properties.Settings.Default["ConnectionString"] = connection_string;
                Properties.Settings.Default.Save();
            }

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            this.ReceivedProductsDataGrid.Items.Clear();
            this.StorageProductsDataGrid.Items.Clear();
            this.SoldProductsDataGrid.Items.Clear();

            var received_products = StorageDB.QueryReceivedProductRecords();
            var storage_products = StorageDB.QueryStorageProductRecords();
            var sold_products = StorageDB.QuerySoldProductRecords();

            foreach (var rp in received_products)
            {
                this.ReceivedProductsDataGrid.Items.Add(rp);
            }

            foreach (var sp in storage_products)
            {
                this.StorageProductsDataGrid.Items.Add(sp);
            }

            foreach (var sdp in sold_products)
            {
                this.SoldProductsDataGrid.Items.Add(sdp);
            }
        }

        private void AddReceivedRecordButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddRecordWindow();
            if (w.ShowDialog() ?? false)
            {
                var rp = w.ReceivedProduct;
                this.ReceivedProductsDataGrid.Items.Add(rp);
            }
        }

        private void ReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var w = new ReportWindow();
            w.Show();
        }

        private void MoveSelectedProducts(DataGrid from_datagrid, DataGrid to_datagrid, Action<ProductRecord> move_action)
        {
            var selected_products = from_datagrid.SelectedItems;
            foreach (ProductRecord sp in selected_products)
            {
                move_action(sp);
                to_datagrid.Items.Add(sp);
            }

            while (from_datagrid.SelectedItem != null)
            {
                int i = from_datagrid.SelectedIndex;
                from_datagrid.Items.RemoveAt(i);
            }
        }
        private void ReceivedToStorageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedProducts(ReceivedProductsDataGrid, StorageProductsDataGrid, StorageDB.MoveToStorage);
        }

        private void StorageToSoldMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedProducts(StorageProductsDataGrid, SoldProductsDataGrid, StorageDB.MoveToSold);
        }

        private void LoadFromDatabaseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadDataFromDatabase();
        }

        private void AddInReceivedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddRecordWindow();
            if (w.ShowDialog() ?? false)
            {
                var rp = w.ReceivedProduct;
                this.ReceivedProductsDataGrid.Items.Add(rp);
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
