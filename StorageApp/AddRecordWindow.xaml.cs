using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StorageApp
{
    /// <summary>
    /// Interaction logic for AddRecordWindow.xaml
    /// </summary>
    public partial class AddRecordWindow : Window
    {

        public ProductRecord ReceivedProduct { get; set; }
        public AddRecordWindow()
        {
            InitializeComponent();
            this.ReceivedProduct = null;
            this.ProductReceivedDatePicker.Text = DateTime.Now.ToString();
            this.ProductNameComboBox.ItemsSource = StorageDB.QueryProductsNames();
            this.ProductNameComboBox.SelectedIndex = 0;
            this.ProductNameTextBox.Text = this.ProductNameComboBox.SelectedItem?.ToString();
            this.ProductNameComboBox.SelectionChanged += ProductNameComboBox_SelectionChanged;
        }

        private void ProductNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ProductNameTextBox.Text = this.ProductNameComboBox.SelectedItem.ToString();
        }

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ProductReceivedDatePicker.Text == "")
            {
                MessageBox.Show("Enter date!!!");
                return;
            }
            else if (this.ProductNameTextBox.Text == "")
            {
                MessageBox.Show("Enter product name!!!");
                return;
            }
            else if (this.ProductPriceTextBox.Text == "")
            {
                MessageBox.Show("Enter product price!!!");
                return;
            }

            DateTime received_date;
            decimal price; 
            if (!DateTime.TryParse(this.ProductReceivedDatePicker.Text, out received_date))
            {
                MessageBox.Show("Date parsing failed!!!");
                return;
            } 
            else if (!decimal.TryParse(this.ProductPriceTextBox.Text, out price))
            {
                MessageBox.Show("Price parsing failed!!!");
                return;
            }

            ProductRecord new_product = new ProductRecord(){
                Price = price,
                ProductName = this.ProductNameTextBox.Text,
                Date = received_date
            };


            // Add to database
            try
            {
                StorageDB.AddNewProduct(new_product);
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
                return;
            }

            this.ReceivedProduct = new_product;
            this.DialogResult = true;
        }
    }
}
