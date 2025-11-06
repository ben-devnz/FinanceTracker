namespace FinanceTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }
        public Decimal Amount { get; set; }
        public string Category { get; set; }

        public Expense()
        {
            Description = string.Empty;
            Amount = 0m;
            Category = string.Empty;

        }
    }
}
