using Core.Models;
using Core.Repositories;
using WebApi.Options;

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
    private readonly INotificationRepository notificationRepository;
    private readonly TimeProvider timeProvider;
    private readonly RecurrentActionServiceOptions options;

    public RecurrentActionService(
        ILogger<RecurrentActionService> logger,
        IServiceMetadataRepository metadataRepository,
        IRepository<Expense> expenseRepository,
        IRepository<Income> incomeRepository,
        IRepository<RecurringAction> recurringRepository,
        INotificationRepository notificationRepository,
        TimeProvider timeProvider,
        RecurrentActionServiceOptions options)
    {
        this.logger = logger;
        this.metadataRepository = metadataRepository;
        this.expenseRepository = expenseRepository;
        this.incomeRepository = incomeRepository;
        this.recurringRepository = recurringRepository;
        this.notificationRepository = notificationRepository;
        this.timeProvider = timeProvider;
        this.options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("{ServiceName} is starting.", ServiceName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = this.timeProvider.GetUtcNow();
            this.logger.LogInformation("Task executing at: {Time}", now);

            var metadata = await this.metadataRepository.GetByNameAsync(ServiceName, stoppingToken);

            if (metadata is null || metadata.UpdatedAt.Date != now.Date)
            {
                var recurring = await this.recurringRepository.GetAllAsync(stoppingToken);

                await this.ProcessRecurringActions(
                    recurring,
                    RecurringType.Expense,
                    "Expense",
                    (action, occurrenceDate) => new Expense
                    {
                        Name = action.Name,
                        Amount = action.Amount,
                        Currency = action.Currency,
                        Description = action.Description,
                        ActionedAt = occurrenceDate,
                    },
                    this.expenseRepository,
                    stoppingToken);

                await this.ProcessRecurringActions(
                    recurring,
                    RecurringType.Income,
                    "Income",
                    (action, occurrenceDate) => new Income
                    {
                        Name = action.Name,
                        Amount = action.Amount,
                        Currency = action.Currency,
                        Description = action.Description,
                        ActionedAt = occurrenceDate,
                    },
                    this.incomeRepository,
                    stoppingToken);

                await this.metadataRepository.UpsertAsync(ServiceMetadata, stoppingToken);
            }
            else
            {
                this.logger.LogInformation("Today's action has already been executed previously.");
            }

            await Task.Delay(this.options.ExecutionIntervalTimeSpan, stoppingToken);
        }

        this.logger.LogInformation("{ServiceName} is stopping.", ServiceName);
    }

    private async Task ProcessRecurringActions<TEntity>(
        IEnumerable<RecurringAction> recurringActions,
        RecurringType type,
        string entityTypeName,
        Func<RecurringAction, DateTime, TEntity> createEntity,
        IRepository<TEntity> entityRepository,
        CancellationToken stoppingToken)
        where TEntity : Entity
    {
        var now = this.timeProvider.GetUtcNow();
        var dueActions = GetDueActions(recurringActions, type, now);
        var processedCount = 0;
        var entityTypeLower = entityTypeName.ToLowerInvariant();

        foreach (var action in dueActions)
        {
            try
            {
                while (action.RecurringAt.Date <= now.Date)
                {
                    var occurrenceDate = action.RecurringAt;
                    var previousRecurringAt = action.RecurringAt;

                    AdvanceAction(action, now);

                    if (action.RecurringAt <= previousRecurringAt)
                    {
                        this.logger.LogError(
                            "Recurring {EntityType} action '{ActionName}' ({ActionId}) did not advance past {RecurringAt} (IntervalValue={IntervalValue}); stopping catch-up to avoid an infinite loop.",
                            entityTypeLower, action.Name, action.Id, action.RecurringAt, action.IntervalValue);
                        break;
                    }

                    var entity = createEntity(action, occurrenceDate);

                    var createdEntity = await entityRepository.CreateAsync(entity, stoppingToken);
                    await this.recurringRepository.UpdateAsync(action, stoppingToken);
                    await this.notificationRepository.CreateAsync(new Notification
                    {
                        Type = $"Recurring{entityTypeName}Processed",
                        Title = $"Recurring {entityTypeLower} added",
                        Message = $"Recurring {entityTypeLower} \"{action.Name}\" of {action.Amount:C} was added for {occurrenceDate:d}.",
                        EntityType = entityTypeName,
                        EntityId = createdEntity.Id,
                    }, stoppingToken);
                    this.logger.LogInformation("Processed {EntityType} action '{ActionName}' for {OccurrenceDate}.", entityTypeLower, action.Name, occurrenceDate);
                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to process recurring {EntityType} action '{ActionName}' ({ActionId}).", entityTypeLower, action.Name, action.Id);
            }
        }

        this.logger.LogInformation("Processed {Count} {EntityType} actions.", processedCount, entityTypeLower);
    }

    private static List<RecurringAction> GetDueActions(IEnumerable<RecurringAction> recurringActions, RecurringType type, DateTimeOffset now) =>
        recurringActions.Where(x => x.IsActive && x.Type == type && x.StartAt.Date <= now.Date && x.RecurringAt.Date <= now.Date).ToList();

    private static void AdvanceAction(RecurringAction action, DateTimeOffset now)
    {
        action.LastExecutedAt = now.DateTime;

        var anchor = action.RecurringAt;

        action.RecurringAt = action.RecurrenceType switch
        {
            RecurrenceType.Daily => anchor.AddDays(action.IntervalValue),
            RecurrenceType.Weekly => anchor.AddDays(action.IntervalValue * 7),
            RecurrenceType.Monthly when action.DayOfMonth.HasValue => AddMonthsClampedToDay(anchor, action.IntervalValue, action.DayOfMonth.Value),
            RecurrenceType.Monthly when !action.DayOfMonth.HasValue => anchor.AddMonths(action.IntervalValue),
            RecurrenceType.Yearly => anchor.AddYears(action.IntervalValue),
            _ => throw new NotSupportedException(),
        };
    }

    private static DateTime AddMonthsClampedToDay(DateTime anchor, int months, int dayOfMonth)
    {
        var target = anchor.AddMonths(months);
        var day = Math.Min(dayOfMonth, DateTime.DaysInMonth(target.Year, target.Month));

        return new DateTime(target.Year, target.Month, day, anchor.Hour, anchor.Minute, anchor.Second, anchor.Kind);
    }
}
