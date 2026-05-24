namespace WebApi.Models
{
    public class CreateExpenseRequest
    {
        public string? CategoryId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? Location { get; set; }

        public string? ReceiptImage { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the action was performed,
        /// not necessary when its added.
        /// </summary>
        public string? ActionedAt { get; set; }
    }
}
