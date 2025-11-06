using FinanceTracker.Models;
using Microsoft.Data.Sqlite;

namespace FinanceTracker.Services
{
    public class SQLiteExpenseRepository : IExpenseRepository
    {
        // FIELDS

        private readonly string _connectionString;

        // CONSTRUCTOR

        public SQLiteExpenseRepository(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
            SeedDataIfEmpty();
        }

        private void SeedDataIfEmpty()
        {
            var existingExpenses = GetAllExpenses();
            if (existingExpenses.Count == 0)
            {
                var seedExpenses = new List<Expense>
                {
                    new Expense { Description = "Grocery shopping at Countdown", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-15)), Amount = 145.50m, Category = "Groceries" },
                    new Expense { Description = "Fuel - BP Station", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-14)), Amount = 85.00m, Category = "Transportation" },
                    new Expense { Description = "Netflix subscription", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-12)), Amount = 20.00m, Category = "Entertainment" },
                    new Expense { Description = "Electricity bill", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)), Amount = 180.00m, Category = "Utilities" },
                    new Expense { Description = "Coffee at local cafe", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-9)), Amount = 6.00m, Category = "Food & Drink" },
                    new Expense { Description = "Gym membership", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-8)), Amount = 60.00m, Category = "Health & Fitness" },
                    new Expense { Description = "Online shopping - Amazon", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)), Amount = 95.50m, Category = "Shopping" },
                    new Expense { Description = "Restaurant dinner", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), Amount = 75.50m, Category = "Food & Drink" },
                    new Expense { Description = "Internet bill", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-4)), Amount = 90.75m, Category = "Utilities" },
                    new Expense { Description = "Pharmacy - prescriptions", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)), Amount = 35.75m, Category = "Healthcare" },
                    new Expense { Description = "Grocery shopping at New World", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), Amount = 120.25m, Category = "Groceries" },
                    new Expense { Description = "Movie tickets", Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), Amount = 40.00m, Category = "Entertainment" },
                };

                foreach (var expense in seedExpenses)
                {
                    AddExpense(expense);
                }
            }
        }

        private void InitializeDatabase()
        {
            using var connection = OpenConnection();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT NOT NULL,
                    Date TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Category TEXT NOT NULL
            )";
            command.ExecuteNonQuery();
        }

        // PUBLIC METHODS

        public List<Expense> GetAllExpenses()
        {
            var expenses = new List<Expense>();

            using var connection = OpenConnection();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Expenses";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                expenses.Add(BuildExpenseFromReader(reader));
            }
            return expenses;
        }

        public void AddExpense(Expense expense)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Expenses (Description, Date, Amount, Category)
                VALUES ($description, $date, $amount, $category)
            ";
            command.Parameters.AddWithValue("$description", expense.Description);
            command.Parameters.AddWithValue("$date", expense.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("$amount", expense.Amount);
            command.Parameters.AddWithValue("$category", expense.Category);

            command.ExecuteNonQuery();

        }

        public void RemoveExpense(int id)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Expenses WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        public void UpdateExpense(Expense expense)
        {
            using var connection = OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Expenses
                SET Description = $description,
                    Date = $date,
                    Amount = $amount,
                    Category = $category
                WHERE Id = $id
            ";
            command.Parameters.AddWithValue("$description", expense.Description);
            command.Parameters.AddWithValue("$date", expense.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("$amount", expense.Amount);
            command.Parameters.AddWithValue("$category", expense.Category);
            command.Parameters.AddWithValue("$id", expense.Id);
            command.ExecuteNonQuery();
        }

        // PRIVATE HELPER FUNCTIONS

        private SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private Expense BuildExpenseFromReader(SqliteDataReader reader)
        {
            return new Expense
            {
                Id = reader.GetInt32(0),
                Description = reader.GetString(1),
                Date = DateOnly.Parse(reader.GetString(2)),
                Amount = reader.GetDecimal(3),
                Category = reader.GetString(4),
            };
        }
    }
}
