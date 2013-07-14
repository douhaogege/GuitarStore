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
            NHibernateBase nhb = new NHibernateBase();
            nhb.Initialize("NHibernate.GuitarStore");
            //PopulateComboBox();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateComboBox();

            NHibernateBase nhb = new NHibernateBase();
            List<Inventory> list = (List<Inventory>)nhb.ExecuteICriteria<Inventory>();
            //NHibernateInventory nbi = new NHibernateInventory();
            //List<Inventory> list = (List<Inventory>)nbi.ExecuteICriteriaOrderBy("Builder");

            dgInventory.ItemsSource = list;
            if (list != null)
            {
                dgInventory.Columns[0].Visibility = Visibility.Hidden;
                dgInventory.Columns[1].Visibility = Visibility.Hidden;
                dgInventory.Columns[8].Visibility = Visibility.Hidden;

            }

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
                dgInventory.Columns[0].Visibility = Visibility.Hidden;
                dgInventory.Columns[1].Visibility = Visibility.Hidden;
                dgInventory.Columns[7].Visibility = Visibility.Hidden;
            }
            PopulateComboBox();
        }
    }
}
