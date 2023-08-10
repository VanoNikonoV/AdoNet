using System.Data;
using System.Windows;

namespace AdoNet.View
{
    /// <summary>
    /// Логика взаимодействия для SelectProductWindow.xaml
    /// </summary>
    public partial class SelectProductWindow : Window
    {
        public DataTable AllProductsCustomer { get;}

        public DataRowView SelectedCustomer { get; }

        public SelectProductWindow(DataTable allProductsCustomer, DataRowView selectedCustomer)
        {
            this.AllProductsCustomer = allProductsCustomer;

            this.SelectedCustomer = selectedCustomer;
            
            InitializeComponent();

            this.Title = $"Выборка продуктов для {SelectedCustomer.Row[2]} {SelectedCustomer.Row[1]}";

            gridViewAllProducts.DataContext = AllProductsCustomer;
        }
    }
}
