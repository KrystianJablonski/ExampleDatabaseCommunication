using Model;
using System;
using ViewModel.MVVM;

namespace ViewModel
{
    /// <summary>
    /// Implementation of ViewModel component to use in the edit product window. 
    /// </summary>
    public class EditProductViewModel : ViewModelBase
    {
        /// <summary>
        /// Construct ViewModel based on parameters.
        /// </summary>
        /// <param name="editingProduct">Product to edit</param>
        /// <param name="onEditEnded">Actions to invoke when editing ends</param>
        public EditProductViewModel(Product editingProduct, params Action<Product>[] onEditEnded)
        {
            _editingProduct = editingProduct;
            _name = editingProduct.Name;
            _count = editingProduct.Count;
            _value = editingProduct.Value;

            foreach (Action<Product> action in onEditEnded)
            {
                _onEditEndedEvent += action;
            }
            AcceptChangeCommand = new ViewModelCommand(AcceptChange);
        }

        #region Private  ViewModel elements

        private event Action<Product> _onEditEndedEvent;
        private Product _editingProduct;

        #endregion

        #region Public binding properties

        /// <summary>
        /// Name binding property
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Count binding property
        /// </summary>
        public int Count
        {
            get => _count;
            set
            {
                if (value >= 0)
                    _count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        /// <summary>
        /// Value binding property
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                if (value >= 0)
                    _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        // Accept change binding command
        public ViewModelCommand AcceptChangeCommand { get; init; }

        #endregion

        #region Private binding fields

        private string _name;
        private int _count;
        private double _value;

        #endregion

        #region ViewModel actions

        /// <summary>
        /// On accept change command execute handler.
        /// Assigns changed values to the product and invokes <seealso cref="_onEditEndedEvent"/>.
        /// </summary>
        private void AcceptChange()
        {
            _editingProduct.Name = _name;
            _editingProduct.Count = _count;
            _editingProduct.Value = _value;
            _onEditEndedEvent?.Invoke(_editingProduct);
        }

        #endregion
    }
}
