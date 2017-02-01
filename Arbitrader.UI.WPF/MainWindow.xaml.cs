using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Arbitrader.UI.WPF.ViewModel;

namespace Arbitrader.UI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DependencyProperty ActiveOrdersProperty = DependencyProperty.Register("ActiveOrders", typeof(ObservableCollection<ActiveOrder>), typeof(MainWindow));

        public ObservableCollection<ActiveOrder> ActiveOrders
        {
            get
            { return (ObservableCollection<ActiveOrder>)this.GetValue(ActiveOrdersProperty); }
            set
            { this.SetValue(ActiveOrdersProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.ActiveOrders = new ObservableCollection<ActiveOrder>();
            this.dgActiveOrders.ItemsSource = this.ActiveOrders;
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            var newOrder = new ActiveOrder()
            {
                Item = this.txtItem.Text,
                Direction = this.cmbDirection.SelectedItem.ToString() == "Buy" ? Direction.Buy : Direction.Sell,
                Quantity = Int32.Parse(this.txtQuantity.Text),
                Price = Int32.Parse(this.txtPrice.Text),
                SellAt = Int32.Parse(this.txtSellAt.Text),
                ExpectedROI = Double.Parse(this.txtExpectedROI.Text)
            };

            this.ActiveOrders.Add(newOrder);
        }
    }
}
