using FinanceTracker.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FinanceTracker.ViewModels
{
    class AddExpenseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // FIELDS

        private string _description;
        private string _category;
        private decimal _amount;
        private DateTime? _selectedDate;
        private Window _window;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
                (SaveExpenseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged();
                (SaveExpenseCommand as RelayCommand)?.RaiseCanExecuteChanged();

            }
        }

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                OnPropertyChanged();
                (SaveExpenseCommand as RelayCommand)?.RaiseCanExecuteChanged();

            }
        }

        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                (SaveExpenseCommand as RelayCommand)?.RaiseCanExecuteChanged();

            }
        }

        // COMMANDS

        public ICommand SaveExpenseCommand { get; set; }
        public ICommand CancelExpenseCommand { get; set; }

        // CONSTRUCTOR

        public AddExpenseViewModel(Window window)
        {
            _window = window;
            _description = string.Empty;
            _category = string.Empty;
            _amount = 0m;
            _selectedDate = null; // Force user to select

            // Initialize Commmands
            SaveExpenseCommand = new RelayCommand(
                executeMethod: (param) => SaveExpense(),
                CanExecuteMethod: (param) => AllFieldsComplete()
            );
            CancelExpenseCommand = new RelayCommand(
                executeMethod: (param) => CancelExpense(),
                CanExecuteMethod: (param) => true
            );
        }

        // PRIVATE METHODS

        private bool AllFieldsComplete()
        {
            return !string.IsNullOrWhiteSpace(Description)
                && !string.IsNullOrWhiteSpace(Category)
                && Amount > 0
                && SelectedDate.HasValue;
        }

        private void SaveExpense()
        {
            if (!AllFieldsComplete())
            {
                MessageBox.Show
                (
                    "Please fill in all fields before saving.",
                    "Incomplete Form",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            _window.DialogResult = true;
            _window.Close();
        }
        private void CancelExpense()
        {
            _window.DialogResult = false;
            _window.Close();
        }


    }
}
