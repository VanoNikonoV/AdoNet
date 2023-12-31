﻿using System.Data;
using System.Windows;

namespace AdoNet.View
{
    /// <summary>
    /// Окно для добавления нового продукта
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private DataRow _newProduct;

        public AddProductWindow()
        {
            InitializeComponent();
        }
        public AddProductWindow(DataRow newProduct) :this()
        {
            _newProduct = newProduct;
            E_mailTextBox.Text = newProduct[1].ToString();
        }

        private void EnterButton(object sender, RoutedEventArgs e)
        {
            _newProduct[3] = NameProductTextBox.Text;
            _newProduct[2] = CodeProductTextBox.Text;
            this.DialogResult = true;
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
