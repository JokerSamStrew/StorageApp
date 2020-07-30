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
using System.Xml.Schema;

namespace StorageApp
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();
            this.AllRadioButton.IsChecked = true;

        }

        enum Status { ALL, RECIEVED, STORAGE, SOLD }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Status status;
            if (this.ReceivedRadioButton.IsChecked ?? false){
                status = Status.RECIEVED;
            } else if (this.StorageRadioButton.IsChecked ?? false) {
                status = Status.STORAGE;
            } else if (this.SoldRadioButton.IsChecked ?? false) {
                status = Status.SOLD;
            } else {
                status = Status.ALL;
            }

            DateTime from_date;
            if (this.FromDatePicker.Text == "")
            {
                from_date = DateTime.MinValue;
            }
            else if (!DateTime.TryParse(this.FromDatePicker.Text, out from_date))
            {
                MessageBox.Show("Bad \"from date\" input!!!");
                return;
            }

            DateTime to_date;
            if (this.ToDatePicker.Text == "")
            {
                to_date = DateTime.MaxValue;
            }
            else if (!DateTime.TryParse(this.ToDatePicker.Text, out to_date))
            {
                MessageBox.Show($"Bad \"to date\" input!!!");
                return;
            }

            //MessageBox.Show($"{to_date}; {from_date};   {to_date.CompareTo(from_date)}");
            if (to_date.CompareTo(from_date) < 0)
            {
                MessageBox.Show("\"to date\" earlier than \"from date\"  input!!!");
                return;
            }

            this.ReportDataGrid.Items.Clear();
            List<ProductRecord> report_records = null;
            decimal sum = -1;
            switch(status)
            {
                case Status.ALL: 
                    report_records = StorageDB.QueryAllProductRecords(from_date, to_date);
                    sum = StorageDB.CountAllProductSum(from_date, to_date);
                    break;
                case Status.RECIEVED: 
                    report_records = StorageDB.QueryReceivedProductRecords(from_date, to_date);
                    sum = StorageDB.CountReceivedProductSum(from_date, to_date);
                    break;
                case Status.STORAGE: 
                    report_records = StorageDB.QueryStorageProductRecords(from_date, to_date);
                    sum = StorageDB.CountStorageProductSum(from_date, to_date);
                    break;
                case Status.SOLD: 
                    report_records = StorageDB.QuerySoldProductRecords(from_date, to_date);
                    sum = StorageDB.CountSoldProductSum(from_date, to_date);
                    break;
            }
            foreach (var r in report_records)
            {
                this.ReportDataGrid.Items.Add(r);
            }
            this.SumTextBlock.Text = $"Total sum: {sum}";
        }
    }
}
