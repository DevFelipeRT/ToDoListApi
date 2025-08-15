using System.Threading;
using System.Threading.Tasks;
using Domain.Lists.Entities;
using Domain.Lists.ValueObjects;

namespace Domain.Lists.Services.Interfaces
{
    /// <summary>
    /// Contract for scheduling reminders for to-do items.
    /// </summary>
    public interface IReminderSchedulerService
    {
        /// <summary>
        /// Schedules a reminder for the specified to-do item.
        /// </summary>
        /// <param name="item">The to-do item to schedule a reminder for.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ScheduleAsync(ToDoItem item, CancellationToken cancellationToken);

        /// <summary>
        /// Cancels a scheduled reminder for the specified to-do item.
        /// </summary>
        /// <param name="itemId">The unique identifier of the to-do item.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CancelAsync(ToDoItemId itemId, CancellationToken cancellationToken);
    }
}
