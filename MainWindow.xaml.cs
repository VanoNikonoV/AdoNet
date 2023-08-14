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

        private DataSet customerOrders;

        private DataRowView row;

        #endregion

        public MainWindow()
        {
            custAdapter = new SqlDataAdapter();

            ordAdapter = new OleDbDataAdapter();

            customerOrders = new DataSet();

            InitializeComponent();
        }

        /// <summary>
        /// Метод подключения и настройки связи между таблицами из двух разных источников
        /// </summary>
        /// <returns>Task</returns>
        public async Task SchemaSettings()
        {
            using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString))
            {
                try
                {
                    await customerConnection.OpenAsync();

                    SqlCommand command = new SqlCommand($"SELECT * FROM customers", customerConnection);

                    custAdapter.SelectCommand = command;

                    custAdapter.Fill(customerOrders, "customers");
                }
                catch (Exception e) { MessageBox.Show($"SQL " + e.Message); }
            }

            using (OleDbConnection orderConnection = new OleDbConnection(OledBConnectionString))
            {
                try
                {
                    await orderConnection.OpenAsync();

                    OleDbCommand command = new OleDbCommand($"SELECT * FROM orders Order By orders.id_product", orderConnection);

                    ordAdapter.SelectCommand = command;

                    ordAdapter.FillSchema(customerOrders, SchemaType.Source, "oreders");

                    ordAdapter.Fill(customerOrders.Tables[1]);
                }
                catch (Exception e) { MessageBox.Show($"Access " + e.Message); }

            }

            DataRelation customerOrdersRelation =
                customerOrders.Relations.Add("CustOrders",
                customerOrders.Tables[0].Columns[5],   //customerOrders.Tables["customers"].Columns["e_mail"],
                customerOrders.Tables[1].Columns[1]);     //customerOrders.Tables["oreders"].Columns["e_mail"]);

            gridView.DataContext = customerOrders.Tables[0].DefaultView;
            gridViewOrders.DataContext = customerOrders.Tables[1].DefaultView;
        }

        private void DeletCustomerButton(object sender, RoutedEventArgs e)
        {
            this.DeleteCommand();
        }

        /// <summary>
        /// Метод удаления (выделенного в GridView) пользователя из базы данных
        /// </summary>
        /// <returns>Task</returns>
        private async Task DeleteCommand()
        {
            row = gridView.SelectedItem as DataRowView;

            if (row != null)
            {
                using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString))
                {
                    try
                    {
                        await customerConnection.OpenAsync();

                        string sql = "DELETE FROM customers WHERE id_user = @id_user";

                        custAdapter.DeleteCommand = new SqlCommand(sql, customerConnection);

                        custAdapter.DeleteCommand.Parameters.Add("@id_user", SqlDbType.Int, 4, "id_user").Direction
                            = ParameterDirection.InputOutput;

                        row.Row.Delete();

                        custAdapter.Update(customerOrders.Tables[0]);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
            else MessageBox.Show(messageBoxText: "Выберите клиента!",
                             caption: "Ощибка",
                             MessageBoxButton.OK,
                             icon: MessageBoxImage.Error);
        }

        private void ClearTableButton(object sender, RoutedEventArgs e)
        {
            this.ClearTabelCommand();
        }

        /// <summary>
        /// Удаление всех данных из базы данных
        /// </summary>
        /// <returns>Task</returns>
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
                    MessageBox.Show(e.Message);
                }
            }
        }

        /// <summary>
        /// Метод создание и заполения данными таблицы товаров для выбранного клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectProductButton(object sender, RoutedEventArgs e)
        {
            var selectedRow = gridView.SelectedItem as DataRowView;

            if (selectedRow != null)
            {
                #region Создание и заполение данными таблицы выборки
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
                #endregion

                SelectProductWindow AllProductsCuctomer = new SelectProductWindow(ds.Tables[0], selectedRow);

                AllProductsCuctomer.Show();
            }
            else MessageBox.Show(messageBoxText: "Выберите клиента!",
                             caption: "Ощибка",
                             MessageBoxButton.OK,
                             icon: MessageBoxImage.Error);
        }

        /// <summary>
        /// Начало редактирования записи о клиетне
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GVCellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            row = gridView.SelectedItem as DataRowView;

            if (row != null) { row.BeginEdit(); }

            else MessageBox.Show(messageBoxText: "Выберите клиента!",
                             caption: "Ощибка",
                             MessageBoxButton.OK,
                             icon: MessageBoxImage.Error);
        }
      
        private void AddNewCustomerButton(object sender, RoutedEventArgs e)
        {
            AddNewCustomer();
        }

        /// <summary>
        /// Метод доваления нового пользователя
        /// </summary>
        /// <returns></returns>
        private async Task AddNewCustomer() 
        {
            using (SqlConnection customerConnection = new SqlConnection(SqlConnectionString))
            {
                try
                {
                    await customerConnection.OpenAsync();

                    DataRow dataRow = customerOrders.Tables[0].NewRow();

                    string sql = @"INSERT INTO customers (surname, name, patronymic, telefon, e_mail) 
                                           VALUES (@surname, @name, @patronymic, @telefon, @e_mail) 
                                           SET @id_user = @@IDENTITY";

                    custAdapter.InsertCommand = new SqlCommand(sql, customerConnection);

                    custAdapter.InsertCommand.Parameters.Add("@id_user", SqlDbType.Int, 4, "id_user").Direction = ParameterDirection.Output;
                    custAdapter.InsertCommand.Parameters.Add("@surname", SqlDbType.NVarChar, 50, "surname");
                    custAdapter.InsertCommand.Parameters.Add("@name", SqlDbType.NVarChar, 50, "name");
                    custAdapter.InsertCommand.Parameters.Add("@patronymic", SqlDbType.NVarChar, 50, "patronymic");
                    custAdapter.InsertCommand.Parameters.Add("@telefon", SqlDbType.NVarChar, 50, "telefon");
                    custAdapter.InsertCommand.Parameters.Add("@e_mail", SqlDbType.NVarChar, 50, "e_mail");

                    custAdapter.Update(customerOrders.Tables[0]);

                    NewCustomerWindow newCustomer = new NewCustomerWindow(dataRow);
                    newCustomer.Owner = this;
                    newCustomer.ShowDialog();

                    if (newCustomer.DialogResult.Value)
                    {
                        customerOrders.Tables[0].Rows.Add(dataRow);
                        custAdapter.Update(customerOrders.Tables[0]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddProductButton(object sender, RoutedEventArgs e)
        {
            AddProduct();
        }

        /// <summary>
        /// Метод добавления нового продукта для выбранного клиента
        /// </summary>
        /// <returns></returns>
        private async Task AddProduct()
        {
            var selectedRow = gridView.SelectedItem as DataRowView;

            if (selectedRow != null)
            {
                using (OleDbConnection orderConnection = new OleDbConnection(OledBConnectionString))
                {
                    try
                    {
                        orderConnection.OpenAsync();

                        DataRow dataRow = customerOrders.Tables[1].NewRow();

                        dataRow[1] = selectedRow.Row[5];

                        string oleDd = @"INSERT INTO orders (e_mail, productCode, nameProduct) VALUES (?, ?, ?);";

                        OleDbCommand command = new OleDbCommand(oleDd, orderConnection);

                        command.Parameters.Add("@e_mail", OleDbType.VarChar, 50, "e_mail");
                        command.Parameters.Add("@id_product", OleDbType.Integer, 4, "id_product");
                        command.Parameters.Add("productCode", OleDbType.Integer, 20, "productCode");
                        command.Parameters.Add("nameProduct", OleDbType.VarChar, 50, "nameProduct");

                        ordAdapter.InsertCommand = command;

                        AddProductWindow newProduct = new AddProductWindow(dataRow);
                        newProduct.Owner = this;
                        newProduct.ShowDialog();

                        if (newProduct.DialogResult.Value)
                        {
                            customerOrders.Tables[1].Rows.Add(dataRow);
                            ordAdapter.Update(customerOrders.Tables[1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else MessageBox.Show(messageBoxText: "Выберите клиента!",
                 caption: "Ощибка",
                 MessageBoxButton.OK,
                 icon: MessageBoxImage.Error);
        }

        /// <summary>
        /// Метод верификация пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadAuthorizationWindow(object sender, RoutedEventArgs e)
        {
            var AuthorizationWindow = new AuthorizationWindow();

            AuthorizationWindow.Owner = this;

            AuthorizationWindow.ShowDialog();

            if (AuthorizationWindow.DialogResult == true)
            {
                SchemaSettings();
            }
            else { this.Close(); }
        }
        
        /// <summary>
        ///  Метод подтверждения изменений в данных о клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
