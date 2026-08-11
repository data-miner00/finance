using Core.Models;
using Core.Repositories;

namespace WebApi.Backgrounds;

public class RecurrentActionService : BackgroundService
{
    private const string ServiceName = nameof(RecurrentActionService);
    private readonly ServiceMetadata ServiceMetadata = new() { ServiceName = ServiceName };
    private readonly ILogger<RecurrentActionService> logger;
    private readonly IServiceMetadataRepository metadataRepository;
    private readonly IRepository<Expense> expenseRepository;
    private readonly IRepository<Income> incomeRepository;
    private readonly IRepository<RecurringAction> recurringRepository;
    private readonly TimeProvider timeProvider;

    public RecurrentActionService(
        ILogger<RecurrentActionService> logger,
        IServiceMetadataRepository metadataRepository,
        IRepository<Expense> expenseRepository,
        IRepository<Income> incomeRepository,
        IRepository<RecurringAction> recurringRepository,
        TimeProvider timeProvider)
    {
        this.logger = logger;
        this.metadataRepository = metadataRepository;
        this.expenseRepository = expenseRepository;
        this.incomeRepository = incomeRepository;
        this.recurringRepository = recurringRepository;
        this.timeProvider = timeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("{ServiceName} is starting.", ServiceName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = this.timeProvider.GetUtcNow();
            this.logger.LogInformation("Task executing at: {Time}", now);

            var metadata = await this.metadataRepository.GetByNameAsync(ServiceName, default);

            if (metadata is null || metadata.UpdatedAt.Date != now.Date)
            {
                var recurring = await this.recurringRepository.GetAllAsync(stoppingToken);

                await this.ProcessRecurringExpenses(recurring);
                // provess recurring income

                await this.metadataRepository.UpsertAsync(ServiceMetadata, stoppingToken);
            }

            // Run every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        this.logger.LogInformation("{ServiceName} is stopping.", ServiceName);
    }

    private async Task ProcessRecurringExpenses(IEnumerable<RecurringAction> recurringExpenses)
    {
        var now = this.timeProvider.GetUtcNow();
        var todayExpenseActions = recurringExpenses.Where(x => x.IsActive && x.Type == RecurringType.Expense && x.StartAt.Date <= now.Date && x.RecurringAt.Date == now.Date).ToList();

        foreach (var action in todayExpenseActions)
        {
            try
            {
                var expense = new Expense
                {
                    Name = action.Name,
                    Amount = action.Amount,
                    Description = action.Description,
                    ActionedAt = now.DateTime,
                };

                action.LastExecutedAt = now.DateTime;

                var anchor = action.RecurringAt;

                var nextOccurrenceDate = action.RecurrenceType switch
                {
                    RecurrenceType.Daily => anchor.AddDays(action.IntervalValue),
                    RecurrenceType.Weekly => anchor.AddDays(action.IntervalValue * 7),
                    RecurrenceType.Monthly when action.DayOfMonth.HasValue => AddMonthsClampedToDay(anchor, action.IntervalValue, action.DayOfMonth.Value),
                    RecurrenceType.Monthly when !action.DayOfMonth.HasValue => anchor.AddMonths(action.IntervalValue),
                    RecurrenceType.Yearly => anchor.AddYears(action.IntervalValue),
                    _ => throw new NotSupportedException(),
                };

                action.RecurringAt = nextOccurrenceDate;

                await this.expenseRepository.CreateAsync(expense, default);
                await this.recurringRepository.UpdateAsync(action, default);
                this.logger.LogInformation("Processed action '{ActionName}'.", action.Name);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to process recurring action '{ActionName}' ({ActionId}).", action.Name, action.Id);
            }
        }

        this.logger.LogInformation("Processed {Count} actions.", todayExpenseActions.Count);
    }

    private static DateTime AddMonthsClampedToDay(DateTime anchor, int months, int dayOfMonth)
    {
        var target = anchor.AddMonths(months);
        var day = Math.Min(dayOfMonth, DateTime.DaysInMonth(target.Year, target.Month));

        return new DateTime(target.Year, target.Month, day, anchor.Hour, anchor.Minute, anchor.Second, anchor.Kind);
    }
}
