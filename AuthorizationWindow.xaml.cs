using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Diagnostics;
using System.Configuration;

namespace AdoNet
{
    public partial class AuthorizationWindow : Window
    {
        SqlDataAdapter sqlDataAdapter;

        DataTable table;

        public AuthorizationWindow()
        {
            InitializeComponent();

            sqlDataAdapter = new SqlDataAdapter();

            table = new DataTable();
        }

        private void Button_Click_Enter(object sender, RoutedEventArgs e)
        {
            string login = this.login.Text;
            string password = this.password.Password;

            string querystring = $"select id_user, login_user, password_user from register where login_user = '{login}' and password_user = '{password}'";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(querystring, connection);

                    sqlDataAdapter.SelectCommand = command;

                    sqlDataAdapter.Fill(table);

                    if(table.Rows.Count == 1)
                    {
                        this.DialogResult = true;
                    }
                    else 
                    {
                        MessageBox.Show("Пользователь не зарегестрирован!"); 
                        this.confirmPassword.Visibility = Visibility.Visible;
                        this.enterBottun.IsEnabled = false;
                        this.registrationBottun.Visibility = Visibility.Visible;
                    }

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void Button_Click_Reg(object sender, RoutedEventArgs e)
        {
            this.enterBottun.Visibility = Visibility.Hidden;
            string login = this.login.Text;
            string password = this.password.Password;
            string confirmPassword = this.confirmPassword.Password;

            if(password.Equals(confirmPassword))
            {
                string querystring = $"INSERT INTO register (login_user, password_user) VALUES ('{login}','{password}')";

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString))
                {
                    try
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand(querystring, connection);

                        int w = command.ExecuteNonQuery();

                        this.DialogResult = true;

                }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
            else { MessageBox.Show("Значения паролей не совпадаю!"); }
        }
    }
}
