using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Diagnostics;

namespace AdoNet
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
        }

        public async Task ConnectionSql()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["sql"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString)) // strConnection.ConnectionString
            {
                try
                {
                    await connection.OpenAsync();

                    this.ConnectionString1.Text = connection.ConnectionString;

                    this.Status1.Text = $@"{nameof(connection)} в состоянии:" + $" {connection.State}";
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
        }

        public async Task ConnectionAccess()
        {
            string connectionString2 =
                ConfigurationManager.ConnectionStrings["MSAccess"].ConnectionString;

            using (OleDbConnection connection = new OleDbConnection(connectionString2)) 
            {
                try
                {
                    await connection.OpenAsync();

                    this.ConnectionString2.Text = connection.ConnectionString;

                    this.Status2.Text = $@"{nameof(connection)} в состоянии:" + $" {connection.State}";
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
        }

        private void LoadAuthorizationWindow(object sender, RoutedEventArgs e)
        {
            var AuthorizationWindow = new AuthorizationWindow();

            AuthorizationWindow.Owner = this;

            AuthorizationWindow.ShowDialog();

            if (AuthorizationWindow.DialogResult == true)
            {
                ConnectionSql();
                ConnectionAccess();
            }
            else { this.Close(); }
        }
       
    }
}
