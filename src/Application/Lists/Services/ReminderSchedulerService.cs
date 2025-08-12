using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Domain.Lists;
using Domain.Lists.ValueObjects;
using Domain.Lists.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Lists.Services
{
    /// <summary>
    /// Service responsible for scheduling and managing reminders for to-do items.
    /// This implementation uses an in-memory timer-based system for reminder scheduling.
    /// </summary>
    public sealed class ReminderSchedulerService : IReminderSchedulerService, IDisposable
    {
        private readonly ILogger<ReminderSchedulerService> _logger;
        private readonly ConcurrentDictionary<Guid, Timer> _scheduledReminders;
        private volatile bool _disposed;

        public ReminderSchedulerService(ILogger<ReminderSchedulerService> logger)
        {
            _logger = logger;
            _scheduledReminders = new ConcurrentDictionary<Guid, Timer>();
        }

        /// <summary>
        /// Schedules a reminder for the specified to-do item.
        /// </summary>
        /// <param name="item">The to-do item to schedule a reminder for.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ScheduleAsync(ToDoItem item, CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ReminderSchedulerService));

            var due = item.DueDate?.Value;
            if (!due.HasValue)
            {
                _logger.LogDebug("Item {ItemId} has no due date to schedule", item.Id.Value);
                return;
            }

            var reminderTime = due.Value;
            var currentTime = DateTime.UtcNow;

            if (reminderTime <= currentTime)
            {
                _logger.LogWarning(
                    "Cannot schedule reminder for item {ItemId} - reminder time {ReminderTime} is in the past",
                    item.Id.Value, reminderTime);
                return;
            }

            // Cancel existing reminder if any
            await CancelAsync(item.Id, cancellationToken);

            var delay = reminderTime - currentTime;
            var timer = new Timer(
                callback: _ => TriggerReminder(item),
                state: null,
                dueTime: delay,
                period: Timeout.InfiniteTimeSpan);

            _scheduledReminders.TryAdd(item.Id.Value, timer);

            _logger.LogInformation(
                "Scheduled reminder for item {ItemId} '{Title}' at {ReminderTime} (in {DelayMinutes} minutes)",
                item.Id.Value, item.Title.Value, reminderTime, delay.TotalMinutes);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Cancels a scheduled reminder for the specified to-do item.
        /// </summary>
        /// <param name="itemId">The unique identifier of the to-do item.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CancelAsync(ToDoItemId itemId, CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ReminderSchedulerService));

            if (_scheduledReminders.TryRemove(itemId.Value, out var timer))
            {
                timer?.Dispose();
                _logger.LogInformation("Canceled reminder for item {ItemId}", itemId.Value);
            }
            else
            {
                _logger.LogDebug("No active reminder found for item {ItemId}", itemId.Value);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Triggers the reminder notification for a to-do item.
        /// </summary>
        /// <param name="item">The to-do item for which the reminder is triggered.</param>
        private void TriggerReminder(ToDoItem item)
        {
            try
            {
                _logger.LogInformation(
                    "REMINDER: Task '{Title}' (ID: {ItemId}) is due now!",
                    item.Title.Value, item.Id.Value);

                // Remove from scheduled reminders after triggering
                _scheduledReminders.TryRemove(item.Id.Value, out var timer);
                timer?.Dispose();

                // Extend here (email, push notification, DB persistence, domain events, etc.)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering reminder for item {ItemId}", item.Id.Value);
            }
        }

        /// <summary>
        /// Disposes all scheduled reminders and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var kvp in _scheduledReminders)
            {
                kvp.Value?.Dispose();
            }

            _scheduledReminders.Clear();
            _logger.LogInformation(
                "ReminderSchedulerService disposed with {Count} reminders cleared",
                _scheduledReminders.Count);
        }
    }
}
