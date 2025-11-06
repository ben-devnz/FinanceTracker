using FinanceTracker.Models;

namespace FinanceTracker.Services
{
    public interface IExpenseRepository
    {
        // CRUD OPERATIONS
        List<Expense> GetAllExpenses();
        void AddExpense(Expense expense);
        void RemoveExpense(int id);
        void UpdateExpense(Expense expense);

    }
}
