using Model;
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
using System.Windows.Shapes;
using ViewModel;

namespace MainWindowProject
{
    /// <summary>
    /// Interaction logic for EditProductWindow.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        public EditProductWindow(Product editingProduct, Action<Product> onEditEnded)
        {
            EditProductViewModel editProductViewModel = new EditProductViewModel(editingProduct, onEditEnded, EditProductViewModel_EditingEndedEvent);
            DataContext = editProductViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Close this window when accept change button is pressed
        /// </summary>
        /// <param name="product"></param>
        private void EditProductViewModel_EditingEndedEvent(Product product)
        {
            this.Close();
        }
    }
}
