using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ViewModel.MVVM;

namespace ViewModel
{
    /// <summary>
    /// Implementation of ViewModel component to use in the main window. 
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Default ViewModel constructor.
        /// Initializes neccessery properties.
        /// </summary>
        public MainViewModel()
        {
            MainModel.GenerateExampleCsvFile();
            _model = new MainModel();
            _model.ShowErrorMessageEvent += message => ShowMessageEvent?.Invoke(message);
            LoadCommand = new ViewModelCommand(OnLoadCommandExecute);
            EditCommand = new ViewModelCommand(OnEditCommandExecute, false);
        }

        #region Private ViewModel elements

        private MainModel _model;

        #endregion

        #region Public ViewModel elements

        public delegate string OpenCvsFileDialogDelegate();
        // show message to user event
        public event Action<string> ShowMessageEvent;
        // Close all children windows event. Invokes when loading new csv file succeeded.
        public event Action CloseChildrenWindows;
        // Begin editing product event. Action parameter should be invoked after editing is completed.
        public event Action<Product, Action<Product>> EditProductEvent;
        // Get csv file path event
        public event OpenCvsFileDialogDelegate OpenCvsFileDialogEvent;

        #endregion

        #region Public binding properties

        // Currently loaded products
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                RaisePropertyChanged();
            }
        }
        // Selected product reference in the Products list
        public Product CurrentProduct
        {
            get => _currentProduct;
            set
            {
                _currentProduct = value;
                EditCommand.CanExecuteValue = _currentProduct != null;
            }
        }

        // Bindable commands

        // Load csv file command
        public ViewModelCommand LoadCommand { get; private set; }
        // Edit selected product command
        public ViewModelCommand EditCommand { get; private set; }

        #endregion

        #region Private binding fields

        private Product _currentProduct = null;
        private ObservableCollection<Product> _products;

        #endregion

        #region ViewModel actions

        /// <summary>
        /// On load command execute handler.
        /// Invokes <seealso cref="OpenCvsFileDialogEvent"/>.
        /// </summary>
        private void OnLoadCommandExecute()
        {
            string filePath = OpenCvsFileDialogEvent?.Invoke();
            if (string.IsNullOrEmpty(filePath))
                return;
            // Run loading products if file path exists
            Task.Run(async () =>
            {
                bool succeeded = await _model.LoadProductsAsync(filePath);
                if (succeeded)
                {
                    CloseChildrenWindows?.Invoke();
                    Products = new ObservableCollection<Product>(_model.Products);
                }
            });
        }

        /// <summary>
        /// On edit command execute handler.
        /// Invokes <seealso cref="EditProductEvent"/> with <seealso cref="CurrentProduct"/> as parameter.
        /// </summary>
        private void OnEditCommandExecute()
        {
            EditProductEvent?.Invoke(CurrentProduct, OnEditEndedEventHandler);
        }

        /// <summary>
        /// On product edit ended event handler.
        /// Update <paramref name="editedProduct"/> if it exists in <seealso cref="Products"/>.
        /// </summary>
        /// <param name="editedProduct"></param>
        private void OnEditEndedEventHandler(Product editedProduct)
        {
            if (!Products.Contains(editedProduct))
                return;
            int index = Products.IndexOf(editedProduct);
            // remove and insert edited product on the same place to refresh the MainWindows's list
            if (Products.Remove(editedProduct))
            {
                Products.Insert(index, editedProduct);
                _ = _model.UpdateProductInDatabaseAsync(editedProduct);
            }
        }

        #endregion
    }
}
