using System;
using System.Collections.Generic;
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
using NHibernate.GuitarStore.DataAccess;
using NHibernate.GuitarStore.Common;
using System.Diagnostics;

namespace GuitarStore
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //计算序列化配置的时间
            Stopwatch stopwatchConfiguration = new Stopwatch();
            TimeSpan timespanConfiguration;
            stopwatchConfiguration.Start();

            NHibernateBase nhb = new NHibernateBase();
            nhb.Initialize("NHibernate.GuitarStore");

            stopwatchConfiguration.Stop();
            timespanConfiguration = stopwatchConfiguration.Elapsed;
            System.Diagnostics.Trace.WriteLine("--------------" + timespanConfiguration.TotalSeconds.ToString());
            //PopulateComboBox();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateComboBox();

            PopulateDataGrid();

        }

        private void PopulateComboBox()
        {
            NHibernateBase nhb = new NHibernateBase();
            IList<Guitar> GuitarTypes = nhb.ExecuteICriteria<Guitar>();
            foreach (var item in GuitarTypes)
            {
                Guitar guitar = new Guitar();
                guitar.Id = item.Id;
                guitar.Type = item.Type;
                cbGuitarTypes.DisplayMemberPath = "Type";
                cbGuitarTypes.SelectedValuePath = "Id";
                cbGuitarTypes.Items.Add(guitar);
            }
        }

        private void cbGuitarTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dgInventory.ItemsSource = null;
            Guitar guitar = (Guitar)cbGuitarTypes.SelectedItem;
            Guid guitarType = new Guid(guitar.Id.ToString());
            NHibernateInventory nhi = new NHibernateInventory();
            List<Inventory> list = (List<Inventory>)nhi.ExecuteICriteria(guitarType);
            dgInventory.ItemsSource = list;
            if (list != null)
            {
                //dgInventory.Columns[0].Visibility = Visibility.Hidden;
                //dgInventory.Columns[1].Visibility = Visibility.Hidden;
                //dgInventory.Columns[7].Visibility = Visibility.Hidden;
            }
            PopulateComboBox();
        }

        private void btnViewSQL_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Utils.FormatSQL(), "最近的NHibernateSQL", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventoryItem = (Inventory)dgInventory.SelectedItem;
            Guid item = new Guid(inventoryItem.Id.ToString());
            NHibernateInventory nhi = new NHibernateInventory();
            if (nhi.DeleteInventoryItem(item))
            {
                dgInventory.ItemsSource = null;
                PopulateDataGrid();
                MessageBox.Show("删除成功！");
            }
            else
            {
                MessageBox.Show("删除失败！");
            }
        }

        private void PopulateDataGrid()
        {
            NHibernateBase nhb = new NHibernateBase();
            List<Inventory> list = (List<Inventory>)nhb.ExecuteICriteria<Inventory>();
            //NHibernateInventory nbi = new NHibernateInventory();
            //List<Inventory> list = (List<Inventory>)nbi.ExecuteICriteriaOrderBy("Builder");

            dgInventory.ItemsSource = list;
            if (list != null)
            {
                //dgInventory.Columns[0].Visibility = Visibility.Hidden;
                //dgInventory.Columns[1].Visibility = Visibility.Hidden;
                //dgInventory.Columns[8].Visibility = Visibility.Hidden;

            }
        }
    }
}
