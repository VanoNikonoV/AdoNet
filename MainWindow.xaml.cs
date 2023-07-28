using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Diagnostics;
using System.Data;

namespace AdoNet
{

    public partial class MainWindow : Window
    {
        SqlDataAdapter custAdapter;

        OleDbDataAdapter ordAdapter;

        DataTable table;

        DataSet customerOrders;

        DataRowView row;

        public MainWindow()
        {
            custAdapter = new SqlDataAdapter();

            ordAdapter = new OleDbDataAdapter();

            table = new DataTable();

            customerOrders = new DataSet();

            InitializeComponent();

            Connection();
     
        }

  

        public async Task Connection()
        {
            SqlConnection customerConnection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);        
            try
            {
                await customerConnection.OpenAsync();

                SqlCommand command = new SqlCommand("select * FROM customers", customerConnection);

                custAdapter.SelectCommand = command;

                custAdapter.Fill(customerOrders, "customers");

                #region delete

                string sql = "DELETE FROM customers WHERE id_user = @id";

                custAdapter.DeleteCommand = new SqlCommand(sql, customerConnection);
                custAdapter.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id_user");

                #endregion

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                Debug.WriteLine("Done");
            }


            OleDbConnection orderConnection =
                new OleDbConnection(ConfigurationManager.ConnectionStrings["MSAccess"].ConnectionString);
            
            try
            {
                await orderConnection.OpenAsync();

                OleDbCommand command = new OleDbCommand("select * FROM orders", orderConnection);

                ordAdapter.SelectCommand = command;

                ordAdapter.Fill(customerOrders, "orders");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Access " + e.Message);
            }
            finally
            {
                Debug.WriteLine("Done");
            }
            

            customerOrders.Relations.Add("CustOrders",
                customerOrders.Tables["customers"].Columns["e_mail"],   //customerOrders.Tables[0].Columns[5],
                customerOrders.Tables["orders"].Columns["e_mail"]);     //customerOrders.Tables[1].Columns[1]);

            gridView.DataContext = customerOrders.Tables[0].DefaultView;

            

            //foreach (DataRow pRow in customerOrders.Tables["Customers"].Rows)
            //{
            //    //Debug.WriteLine(pRow["e_mail"]);
            //    foreach (DataRow cRow in pRow.GetChildRows(relation))
            //        Debug.WriteLine("\t" + cRow[1]);
            //}

        }

        private void EditCustomerButton(object sender, RoutedEventArgs e)
        {
            
        }

        private void DeleteCustomerButton(object sender, RoutedEventArgs e)
        {
            row = (DataRowView)gridView.SelectedItem;
            row.Row.Delete();
            custAdapter.Update(customerOrders.Tables[0]);
        }

        //private void LoadAuthorizationWindow(object sender, RoutedEventArgs e)
        //{
        //    var AuthorizationWindow = new AuthorizationWindow();

        //    AuthorizationWindow.Owner = this;

        //    AuthorizationWindow.ShowDialog();

        //    if (AuthorizationWindow.DialogResult == true)
        //    {
        //        //Connection();
        //    }
        //    else { this.Close(); }
        //}

    }
}
