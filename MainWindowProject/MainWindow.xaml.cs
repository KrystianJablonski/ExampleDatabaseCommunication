using Microsoft.Win32;
using System;
using System.Windows;
using ViewModel;

namespace MainWindowProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShowMessageEvent += (s) => MessageBox.Show(s);
                viewModel.EditProductEvent += ViewModel_EditProductHandler;
                viewModel.OpenCvsFileDialogEvent += ViewModel_OpenCvsFileDialogHandler;
                viewModel.CloseChildrenWindows += () => Dispatcher.Invoke(ViewModel_CloseChildrenWindows);
            }
        }

        private string ViewModel_OpenCvsFileDialogHandler()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog .DefaultExt = ".csv";
            openFileDialog .Filter = "CSV Files | *.csv";
            bool? result = openFileDialog.ShowDialog();

            return result == true ? openFileDialog.FileName : null;
        }

        private void ViewModel_EditProductHandler(Model.Product editingProduct, Action<Model.Product> onEditEndedAction)
        {
            EditProductWindow newEditWindow = new EditProductWindow(editingProduct, onEditEndedAction);
            newEditWindow.Owner = this;
            newEditWindow.Show();
        }

        private void ViewModel_CloseChildrenWindows()
        {
            foreach (Window window in OwnedWindows)
                window.Close();
        }
    }
}
