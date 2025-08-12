using System;
using Domain.Accounts;

namespace Domain.Lists.Services.Interfaces
{
    /// <summary>
    /// Contract for transferring a to-do item between lists for a specific user.
    /// </summary>
    public interface IToDoListItemTransferService
    {
        /// <summary>
        /// Transfers a to-do item from its source list to the target list for the specified user.
        /// </summary>
        /// <param name="user">The user who owns the lists.</param>
        /// <param name="item">The to-do item to be transferred.</param>
        /// <param name="sourceList">The source to-do list.</param>
        /// <param name="targetList">The target to-do list.</param>
        /// <returns>True if the transfer was successful; otherwise, false.</returns>
        bool TransferToDoItem(
            User user,
            ToDoItem item,
            ToDoList sourceList,
            ToDoList targetList);
    }
}
