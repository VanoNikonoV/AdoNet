using System;
using System.Collections.Generic;
using System.Data;
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

namespace AdoNet.View
{
    /// <summary>
    /// Логика взаимодействия для NewCustomerrWindow.xaml
    /// </summary>
    public partial class NewCustomerWindow : Window
    {
        private DataRow _newCustomer;
        public NewCustomerWindow()
        {
            InitializeComponent();
        }
        public NewCustomerWindow(DataRow newCustomer): this ()
        {
            this._newCustomer = newCustomer;
        }

        private void EnterButton(object sender, RoutedEventArgs e)
        {
            _newCustomer[1] = NameTextBox.Text;
            _newCustomer[2] = SurNameTextBox.Text;
            _newCustomer[3] = PatronymicTextBox.Text;
            _newCustomer[4] = TelefonTextBox.Text;
            _newCustomer[5] = E_mailTextBox.Text;

            this.DialogResult = true;
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
