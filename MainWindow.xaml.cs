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

        public MainWindow()
        {
            custAdapter = new SqlDataAdapter();

            ordAdapter = new OleDbDataAdapter();

            table = new DataTable();

            customerOrders = new DataSet();

            InitializeComponent(); 
        }

        public async Task ConnectionTry()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["sql"].ConnectionString;

            string querySelect = $"select * FROM customers"; //Order By customers.id

            using (SqlConnection customerConnection = new SqlConnection(connectionString)) // strConnection.ConnectionString
            {
                try
                {
                    await customerConnection.OpenAsync();

                    SqlCommand command = new SqlCommand(querySelect, customerConnection);

                    custAdapter.SelectCommand = command;

                    custAdapter.Fill(customerOrders, "customers");

                    gridView.DataContext = customerOrders.Tables[0];
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                finally
                {
                    Debug.WriteLine("Done");
                }
            }

            string connectionString2 =
                ConfigurationManager.ConnectionStrings["MSAccess"].ConnectionString;

            querySelect = $"select * FROM orders";

            using (OleDbConnection orderConnection = new OleDbConnection(connectionString2))
            {
                try
                {
                    await orderConnection.OpenAsync();

                    OleDbCommand command = new OleDbCommand(querySelect, orderConnection);

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
            }

            //DataRelation relation = customerOrders.Relations.Add("CustOrders",
            //    customerOrders.Tables["Customers"].Columns["CustomerID"],
            //    customerOrders.Tables["Orders"].Columns["CustomerID"]);
        }



        private void LoadAuthorizationWindow(object sender, RoutedEventArgs e)
        {
            var AuthorizationWindow = new AuthorizationWindow();

            AuthorizationWindow.Owner = this;

            AuthorizationWindow.ShowDialog();

            if (AuthorizationWindow.DialogResult == true)
            {
                ConnectionTry();
               
            }
            else { this.Close(); }
        }
       
    }
}
