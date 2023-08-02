using System.Data;
using System.Windows;

namespace AdoNet.View
{
    /// <summary>
    /// Логика взаимодействия для SelectProductWindow.xaml
    /// </summary>
    public partial class SelectProductWindow : Window
    {
        private DataTable allProductsCustomer;
        public SelectProductWindow(DataTable AllProductsCustomer)
        {
            allProductsCustomer = AllProductsCustomer;
            
            InitializeComponent();

            gridViewAllProducts.DataContext = AllProductsCustomer;
        }

        
    }
}
