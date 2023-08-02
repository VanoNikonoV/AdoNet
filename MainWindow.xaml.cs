using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using AdoNet.View;

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
            DataRelation customerOrdersRelation = 
                customerOrders.Relations.Add("CustOrders",
                customerOrders.Tables["customers"].Columns[5],   //customerOrders.Tables[0].Columns["e_mail"],
                customerOrders.Tables["orders"].Columns[1]);     //customerOrders.Tables[1].Columns["e_mail"]);

            //ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint("FK_customers",
            //    customerOrders.Tables[0].Columns["e_mail"], customerOrders.Tables[1].Columns["e_mail"]);
            //foreignKeyConstraint.DeleteRule = Rule.Cascade;
            //foreignKeyConstraint.UpdateRule = Rule.Cascade;
            //foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;

            //customerOrders.Tables[0].Constraints.Add(foreignKeyConstraint);

            gridView.DataContext = customerOrders.Tables[0];
            gridViewOrders.DataContext = customerOrders.Tables[1];


            //foreach (DataRow custRow in customerOrders.Tables["customers"].Rows)
            //{
            //    Console.WriteLine(custRow["e_mail"]);
            //    foreach (DataRow orderRow in custRow.GetChildRows(customerOrders.Relations[0]))
            //        Console.WriteLine("\t" + orderRow["e_mail"]);
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

                    string sql = "DELETE FROM [dbo].[customers]";

                    custAdapter.DeleteCommand = new SqlCommand(sql, customerConnection);

                    //int x = cmd.ExecuteNonQuery();

                    //customerOrders.Tables[0].Clear();

                    //rows = (DataRowCollection)gridView.ItemsSource;

                    //rows.Clear();

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private void SelectProductButton(object sender, RoutedEventArgs e)
        {
            var selectedRow = (DataRowView)gridView.SelectedItem;

            DataTable allProducts = new DataTable("allProducts");

            DataColumn column;

            DataRow row;

            // первая колонка
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "id_product";
            column.ReadOnly = true;
            column.Unique = true;
            allProducts.Columns.Add(column);

            //вторая колонка
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "e_mail";
            column.ReadOnly = true;
            column.Unique = false;
            allProducts.Columns.Add(column);

            //трейтья колонка
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "productCode";
            column.ReadOnly = true;
            column.Unique = true;
            allProducts.Columns.Add(column);

            //четвертая колонка
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "nameProduct";
            column.ReadOnly = true;
            column.Unique = true;
            allProducts.Columns.Add(column);

            DataSet ds = new DataSet();

            ds.Tables.Add(allProducts);

            foreach (DataRow custRow in customerOrders.Tables["customers"].Rows)
            {
                if (selectedRow[0].Equals(custRow[0]))
                {
                    foreach (DataRow orderRow in custRow.GetChildRows(customerOrders.Relations[0]))
                    {
                        row = allProducts.NewRow();
                        row["id_product"] = orderRow[0];
                        row["e_mail"] = orderRow[1];
                        row["productCode"] = orderRow[2];  
                        row["nameProduct"] = orderRow[3];
                        allProducts.Rows.Add(row);
                    }
                    break;
                }              
            }

            SelectProductWindow AllProductsCuctomer = new SelectProductWindow(ds.Tables[0]);

            AllProductsCuctomer.Show();
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
