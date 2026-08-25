using Core.Models;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services
{
    public sealed class BudgetAlertService : IBudgetAlertService
    {
        private const string BudgetNearLimitType = "BudgetNearLimit";
        private const string BudgetOverBudgetType = "BudgetOverBudget";
        private const decimal NearLimitRatio = 0.8m;

        private readonly CategoryRepository categoryRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly ILogger<BudgetAlertService> logger;

        public BudgetAlertService(
            CategoryRepository categoryRepository,
            INotificationRepository notificationRepository,
            ILogger<BudgetAlertService> logger)
        {
            this.categoryRepository = categoryRepository;
            this.notificationRepository = notificationRepository;
            this.logger = logger;
        }

        public async Task EvaluateAsync(string? categoryName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return;
            }

            try
            {
                var status = await this.categoryRepository.GetMonthlySpendAsync(categoryName, cancellationToken);
                if (status?.BudgetAmount is null || status.BudgetAmount <= 0)
                {
                    return;
                }

                var ratio = status.SpentThisMonth / status.BudgetAmount.Value;
                string? type = ratio > 1.0m ? BudgetOverBudgetType : ratio >= NearLimitRatio ? BudgetNearLimitType : null;
                if (type is null)
                {
                    return;
                }

                if (await this.notificationRepository.ExistsForEntityThisMonthAsync("Category", status.CategoryId, type, cancellationToken))
                {
                    return;
                }

                var isOverBudget = type == BudgetOverBudgetType;
                await this.notificationRepository.CreateAsync(new Notification
                {
                    Type = type,
                    Title = isOverBudget ? "Over budget" : "Near budget limit",
                    Message = isOverBudget
                        ? $"You've spent {status.SpentThisMonth:C} in \"{status.CategoryName}\" this month, over your {status.BudgetAmount:C} budget."
                        : $"You've spent {status.SpentThisMonth:C} of your {status.BudgetAmount:C} budget in \"{status.CategoryName}\" this month.",
                    EntityType = "Category",
                    EntityId = status.CategoryId,
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to evaluate budget alerts for category '{CategoryName}'.", categoryName);
            }
        }
    }
}
