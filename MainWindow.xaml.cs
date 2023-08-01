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
        private SqlDataAdapter custAdapter;

        private OleDbDataAdapter ordAdapter;

        private DataTable table;

        private DataSet customerOrders;

        DataRowView row;

        DataRowCollection rows;

        private readonly string connectionStringSql =
                ConfigurationManager.ConnectionStrings["sql"].ConnectionString;

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
            string connectionString =
                ConfigurationManager.ConnectionStrings["sql"].ConnectionString;

            string querySelect = $"select * FROM customers"; //Order By customers.id

            using (SqlConnection customerConnection = new SqlConnection(connectionString)) 
            {
                try
                {
                    await customerConnection.OpenAsync();

                    SqlCommand command = new SqlCommand(querySelect, customerConnection);

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
            customerOrders.Relations.Add("CustOrders",
                customerOrders.Tables["customers"].Columns["e_mail"],   //customerOrders.Tables[0].Columns[5],
                customerOrders.Tables["orders"].Columns["e_mail"]);     //customerOrders.Tables[1].Columns[1]);

            


            ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint("FK_customers",
                customerOrders.Tables[0].Columns["e_mail"], customerOrders.Tables[1].Columns["e_mail"]);
            foreignKeyConstraint.DeleteRule = Rule.Cascade;
            foreignKeyConstraint.UpdateRule = Rule.Cascade;
            foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;

            //customerOrders.Tables[0].Constraints.Add(foreignKeyConstraint);

            gridView.DataContext = customerOrders.Tables[0];
            gridViewOrders.DataContext = customerOrders.Tables[1];

            //table = customerOrders.Tables[0];

            //foreach (DataRow pRow in customerOrders.Tables["Customers"].Rows)
            //{
            //    //Debug.WriteLine(pRow["e_mail"]);
            //    foreach (DataRow cRow in pRow.GetChildRows(customerOrders.Relations[0]))
            //        Debug.WriteLine("\t" + cRow[1]);
            //}

        }

        private void DeletCustomerButton(object sender, RoutedEventArgs e)
        {
            this.DeleteCommand(this.connectionStringSql);
        }

        private async Task DeleteCommand(string connectionStringSql)
        {
            using (SqlConnection customerConnection = new SqlConnection(connectionStringSql))
            {
                try
                {
                    await customerConnection.OpenAsync();

                    string sql = "DELETE FROM customers WHERE id_user = @id";

                    custAdapter.DeleteCommand = new SqlCommand(sql, customerConnection);

                    custAdapter.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id_user");

                    row = (DataRowView)gridView.SelectedItem;

                    row.Row.Delete();

                    custAdapter.Update(customerOrders.Tables[0]);

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private void ClearTableButton(object sender, RoutedEventArgs e)
        {
            this.ClearTabelCommand(this.connectionStringSql);
        }

        private async Task ClearTabelCommand(string connectionStringSql)
        {
            using (SqlConnection customerConnection = new SqlConnection(connectionStringSql))
            {
                try
                {
                    await customerConnection.OpenAsync();

                    //customerOrders.Tables[0].Clear();

                    string sql = "DELETE FROM customers";

                    custAdapter.DeleteCommand = new SqlCommand(sql, customerConnection);

                    //custAdapter.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id_user");

                    //rows = (DataRowCollection)gridView.ItemsSource;

                    //rows.Clear();

                    custAdapter.Update(customerOrders.Tables[0]);

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
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
