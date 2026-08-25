using System.Threading;
using System.Threading.Tasks;

namespace Core.Services
{
    /// <summary>
    /// Evaluates whether a category's spend this month warrants a budget notification.
    /// </summary>
    public interface IBudgetAlertService
    {
        /// <summary>
        /// Checks the given category's monthly spend against its budget and creates a
        /// near-limit/over-budget notification if warranted and not already raised this month.
        /// </summary>
        /// <param name="categoryName">The category to evaluate; a no-op if null/whitespace.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task EvaluateAsync(string? categoryName, CancellationToken cancellationToken);
    }
}
