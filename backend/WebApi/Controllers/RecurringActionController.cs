using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/recurring")]
    public class RecurringActionController : ControllerBase
    {
        private readonly IRepository<RecurringAction> _repository;

        public RecurringActionController(IRepository<RecurringAction> repository)
        {
            _repository = repository;
        }

        private DateTime CalculateNextExecutionDate(DateTime startAt, RecurrenceType recurrenceType, int intervalValue, int? dayOfMonth)
        {
            var now = DateTime.UtcNow;

            // If start date is in the future or today, return it as the first execution
            if (startAt.Date >= now.Date)
            {
                return startAt;
            }

            // Calculate next execution based on recurrence type
            return recurrenceType switch
            {
                RecurrenceType.Daily => CalculateNextDaily(startAt, intervalValue, now),
                RecurrenceType.Weekly => CalculateNextWeekly(startAt, intervalValue, now),
                RecurrenceType.Monthly => CalculateNextMonthly(startAt, intervalValue, dayOfMonth, now),
                RecurrenceType.Yearly => CalculateNextYearly(startAt, intervalValue, dayOfMonth, now),
                _ => startAt
            };
        }

        private DateTime CalculateNextDaily(DateTime startAt, int intervalValue, DateTime now)
        {
            var next = startAt;
            while (next < now)
            {
                next = next.AddDays(intervalValue);
            }
            return next;
        }

        private DateTime CalculateNextWeekly(DateTime startAt, int intervalValue, DateTime now)
        {
            var next = startAt;
            while (next < now)
            {
                next = next.AddDays(intervalValue * 7);
            }
            return next;
        }

        private DateTime CalculateNextMonthly(DateTime startAt, int intervalValue, int? dayOfMonth, DateTime now)
        {
            var day = dayOfMonth ?? startAt.Day;
            var next = new DateTime(startAt.Year, startAt.Month, 1).AddDays(day - 1);

            // Ensure the day is valid for the month
            int daysInMonth = DateTime.DaysInMonth(next.Year, next.Month);
            if (day > daysInMonth)
            {
                next = new DateTime(next.Year, next.Month, daysInMonth);
            }

            while (next < now)
            {
                next = next.AddMonths(intervalValue);
                // Recalculate for the new month in case day exceeds days in month
                daysInMonth = DateTime.DaysInMonth(next.Year, next.Month);
                if (day > daysInMonth)
                {
                    next = new DateTime(next.Year, next.Month, daysInMonth);
                }
            }
            return next;
        }

        private DateTime CalculateNextYearly(DateTime startAt, int intervalValue, int? dayOfMonth, DateTime now)
        {
            var day = dayOfMonth ?? startAt.Day;
            var month = startAt.Month;
            var next = new DateTime(startAt.Year, month, 1).AddDays(day - 1);

            // Handle edge case for Feb 29
            int daysInMonth = DateTime.DaysInMonth(next.Year, month);
            if (day > daysInMonth)
            {
                next = new DateTime(next.Year, month, daysInMonth);
            }

            while (next < now)
            {
                next = next.AddYears(intervalValue);
                // Recalculate for the new year in case of leap year
                daysInMonth = DateTime.DaysInMonth(next.Year, month);
                if (day > daysInMonth)
                {
                    next = new DateTime(next.Year, month, daysInMonth);
                }
            }
            return next;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurringAction>>> GetAll(CancellationToken cancellationToken)
        {
            var recurringActions = await _repository.GetAllAsync(cancellationToken);
            return Ok(recurringActions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecurringAction>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var recurringAction = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(recurringAction);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<RecurringAction>> Create(CreateRecurringActionRequest request, CancellationToken cancellationToken)
        {
            var nextExecutionDate = CalculateNextExecutionDate(request.StartAt, request.RecurrenceType, request.IntervalValue, request.DayOfMonth);

            var recurringAction = new RecurringAction
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                Amount = request.Amount,
                RecurringAt = nextExecutionDate,
                Type = request.Type,
                StartAt = request.StartAt,
                RecurrenceType = request.RecurrenceType,
                IntervalValue = request.IntervalValue,
                DayOfMonth = request.DayOfMonth,
            };

            var createdRecurringAction = await _repository.CreateAsync(recurringAction, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdRecurringAction.Id }, createdRecurringAction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateRecurringActionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var recurringAction = await _repository.GetByIdAsync(id, cancellationToken);
                recurringAction.Name = request.Name;
                recurringAction.Description = request.Description;
                recurringAction.Amount = request.Amount;
                recurringAction.IsActive = request.IsActive;
                recurringAction.StartAt = request.StartAt;
                recurringAction.RecurrenceType = request.RecurrenceType;
                recurringAction.IntervalValue = request.IntervalValue;
                recurringAction.DayOfMonth = request.DayOfMonth;

                // Recalculate next execution date based on updated properties
                recurringAction.RecurringAt = CalculateNextExecutionDate(request.StartAt, request.RecurrenceType, request.IntervalValue, request.DayOfMonth);

                await _repository.UpdateAsync(recurringAction, cancellationToken);
                return NoContent();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.GetByIdAsync(id, cancellationToken);
                await _repository.DeleteByIdAsync(id, cancellationToken);
                return NoContent();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
