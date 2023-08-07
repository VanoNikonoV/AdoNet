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
        #region Поля
        private readonly string SqlConnectionString =
                ConfigurationManager.ConnectionStrings["sql"].ConnectionString;

        private readonly string OledBConnectionString=
                ConfigurationManager.ConnectionStrings["MSAccess"].ConnectionString;

        private SqlDataAdapter custAdapter;

        private OleDbDataAdapter ordAdapter;

        private DataTable table;

        private DataSet customerOrders;

        DataRowView row;

        DataRowCollection rows;
        #endregion


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
            string querySelect = $"select * FROM customers"; //Order By customers.id

            using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString)) 
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
            }

            using (OleDbConnection orderConnection = new OleDbConnection(OledBConnectionString))
            {
                try
                {
                    await orderConnection.OpenAsync();

                    OleDbCommand command = new OleDbCommand($"select * FROM orders", orderConnection);

                    ordAdapter.SelectCommand = command;

                    ordAdapter.Fill(customerOrders, "orders");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Access " + e.Message);
                }
                
            }
            DataRelation customerOrdersRelation = 
                customerOrders.Relations.Add("CustOrders",
                customerOrders.Tables["customers"].Columns[5],   //customerOrders.Tables[0].Columns["e_mail"],
                customerOrders.Tables["orders"].Columns[1]);     //customerOrders.Tables[1].Columns["e_mail"]);

            gridView.DataContext = customerOrders.Tables[0].DefaultView;
            gridViewOrders.DataContext = customerOrders.Tables[1].DefaultView;
        }

        private void DeletCustomerButton(object sender, RoutedEventArgs e)
        {
            this.DeleteCommand();
        }

        private async Task DeleteCommand()
        {
            using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString))
            {
                try
                {
                    await customerConnection.OpenAsync();

                    string sql = "DELETE FROM customers WHERE id_user = @id_user";

                    custAdapter.DeleteCommand = new SqlCommand(sql, customerConnection);

                    custAdapter.DeleteCommand.Parameters.Add("@id_user", SqlDbType.Int, 4, "id_user").Direction = ParameterDirection.InputOutput;

                    row = (DataRowView)gridView.SelectedItem;

                    row.Row.Delete();

                    custAdapter.Update(customerOrders.Tables[0]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void ClearTableButton(object sender, RoutedEventArgs e)
        {
            this.ClearTabelCommand();
        }

        private async Task ClearTabelCommand()
        {
            using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString))
            {
                try
                {
                    await customerConnection.OpenAsync();

                    string sql = "DELETE FROM [dbo].[customers]";

                    SqlCommand command = new SqlCommand(sql, customerConnection);

                    int x = command.ExecuteNonQuery();

                    customerOrders.Tables[1].Clear();
                    customerOrders.Tables[0].Clear();
                    custAdapter.Update(customerOrders.Tables[0]);
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

        /// <summary>
        /// Вызов методя для редактирования клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditcustomerButton(object sender, RoutedEventArgs e)
        {
            
        }

        private async Task UpdateCustomerButton()
        {
            using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString))
            {
                try
                {
                    await customerConnection.OpenAsync();

                    string sql = "UPDATE customers SET" +
                        "id_user = @id_user," +
                        "surname = @surname," +
                        "name = @name," +
                        "patronymic = @patronymic," +
                        "telefon =@telefon," +
                        "e_mail = @e_mail";

                    custAdapter.UpdateCommand = new SqlCommand(sql, customerConnection);
                    custAdapter.UpdateCommand.Parameters.Add("@id", SqlDbType.Int, 4, "id_user");
                    custAdapter.UpdateCommand.Parameters.Add("@surname", SqlDbType.NVarChar, 4, "surname");
                    custAdapter.UpdateCommand.Parameters.Add("@name", SqlDbType.NVarChar, 4, "name");
                    custAdapter.UpdateCommand.Parameters.Add("@patronymic", SqlDbType.NVarChar, 4, "patronymic");
                    custAdapter.UpdateCommand.Parameters.Add("@telefon", SqlDbType.NVarChar, 4, "telefon");
                    custAdapter.UpdateCommand.Parameters.Add("@e_mail", SqlDbType.NVarChar, 4, "e_mail");

                }
                catch (Exception e)
                {
                    
                    //MessageBox.Show(e.Message);
                }
            }
        }

        private void GVCellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            //var selectedRow = (DataRowView)gridView.SelectedItem;
            row = (DataRowView)gridView.SelectedItem;
            row.BeginEdit();
        }

        private void GVCurrentCellChanged(object sender, EventArgs e)
        {
            if (row == null) return;

            using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString))
            {
                try
                {
                    customerConnection.Open();

                    string sql = @"UPDATE customers SET 
                                    surname = @surname, 
                                    name = @name, 
                                    patronymic = @patronymic, 
                                    telefon = @telefon, 
                                    e_mail = @e_mail 
                                    WHERE id_user = @id_user";

                    custAdapter.UpdateCommand = new SqlCommand(sql, customerConnection);
                    custAdapter.UpdateCommand.Parameters.Add("@id_user", SqlDbType.NVarChar, 4, "id_user");
                    custAdapter.UpdateCommand.Parameters.Add("@surname", SqlDbType.NVarChar, 50, "surname");
                    custAdapter.UpdateCommand.Parameters.Add("@name", SqlDbType.NVarChar, 50, "name");
                    custAdapter.UpdateCommand.Parameters.Add("@patronymic", SqlDbType.NVarChar, 50, "patronymic");
                    custAdapter.UpdateCommand.Parameters.Add("@telefon", SqlDbType.NVarChar, 11, "telefon");
                    custAdapter.UpdateCommand.Parameters.Add("@e_mail", SqlDbType.NVarChar, 50, "e_mail"); 

                    row.EndEdit();

                    custAdapter.Update(customerOrders.Tables[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //MessageBox.Show(ex.Message);
                }
            }

        }

        //-------------------LOAD-----------------------------------------------

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
