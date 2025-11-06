using FinanceTracker.Commands;
using FinanceTracker.Models;
using FinanceTracker.Services;
using FinanceTracker.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FinanceTracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // FIELDS
        private IExpenseRepository _repo;
        private Expense _selectedExpense;

        // PROPERTIES
        public ObservableCollection<Expense> Expenses { get; set; }
        public Expense SelectedExpense
        {
            get { return _selectedExpense; }
            set
            {
                _selectedExpense = value;
                OnPropertyChanged();

                // TODO : Recheck can executes
                (RemoveExpenseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        public decimal TotalExpenses
        {
            get { return Expenses.Sum(e => e.Amount); }
        }

        // COMMANDS
        public ICommand AddExpenseCommand { get; set; }
        public ICommand RemoveExpenseCommand { get; set; }
        // public ICommand UpdateExpenseCommand { get; set; }

        // CONSTRUCTOR
        public MainViewModel()
        {
            // Set db path
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbFolder = Path.Combine(appDataFolder, "ExpenseManager");
            var dbPath = Path.Combine(dbFolder, "expenses.db");

            // Create Folder if it doesn't exist
            Directory.CreateDirectory(dbFolder);

            // Create repository and load data
            _repo = new SQLiteExpenseRepository(dbPath);

            // Initialize Expense Collection
            Expenses = new ObservableCollection<Expense>();
            LoadExpenses();

            // Initialize Commands
            AddExpenseCommand = new RelayCommand(
                executeMethod: (param) => AddExpense(),
                CanExecuteMethod: (param) => true
            );
            RemoveExpenseCommand = new RelayCommand(
                executeMethod: (param) => RemoveExpense(),
                CanExecuteMethod: (param) => SelectedExpense != null
            );
            //UpdateExpenseCommand = new RelayCommand(
            // executeMethod: (param) => UpdateExpense(),
            // CanExecuteMethod: (param) => SelectedExpense != null
            //);
        }

        // PRIVATE METHODS
        private void LoadExpenses()
        {
            Expenses.Clear();
            var expensesFromDb = _repo.GetAllExpenses();
            foreach (var expense in expensesFromDb)
            {
                Expenses.Add(expense);
            }
            OnPropertyChanged(nameof(TotalExpenses));
        }
        private void AddExpense()
        {
            // Create and show the AddExpense window
            var addWindow = new AddExpenseView();

            // Set the owner of the window
            addWindow.Owner = Application.Current.MainWindow;

            // Attach View Model before data ShowDialog
            addWindow.DataContext = new AddExpenseViewModel(addWindow);

            // Show as dialog
            bool? result = addWindow.ShowDialog();

            // If the user clicked save
            if (result == true && addWindow.DataContext is AddExpenseViewModel vm)
            {
                if (!vm.SelectedDate.HasValue) return;

                var newExpense = new Expense
                {
                    Description = vm.Description,
                    Date = DateOnly.FromDateTime(vm.SelectedDate.Value),
                    Amount = vm.Amount,
                    Category = vm.Category,
                };

                // Add expense to repo
                _repo.AddExpense(newExpense);

                // Refresh UI
                LoadExpenses();
            }
        }
        private void RemoveExpense()
        {
            if (SelectedExpense != null)
            {
                _repo.RemoveExpense(SelectedExpense.Id);
                LoadExpenses();
            }
        }
        //private void UpdateExpense()
        //{
        //    if (SelectedExpense == null) return;

        //    var updateWindow = new UpdateExpenseView();
        //    updateWindow.Owner = Application.Current.MainWindow;
        //    updateWindow.DataContext = new UpdateExpenseViewModel(updateWindow, SelectedExpense);
        //    bool? result = updateWindow.ShowDialog();
        //    if (result == true)
        //    {
        //        _repo.UpdateExpense(SelectedExpense);
        //        LoadExpenses();
        //    }
        //}
    }
}
