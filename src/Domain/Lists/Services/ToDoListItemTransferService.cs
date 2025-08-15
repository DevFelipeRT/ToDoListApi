using System.Linq;
using Domain.Accounts.Entities;
using Domain.Lists.Entities;
using Domain.Lists.Services.Interfaces;

namespace Domain.Lists.Services;

/// <summary>
/// Provides functionality to transfer a to-do item between lists for a specific account.
/// </summary>
public class ToDoListItemTransferService : IToDoListItemTransferService
{
    /// <summary>
    /// Transfers a to-do item from its source list to the target list for the specified account.
    /// </summary>
    /// <param name="account">The account who owns the lists.</param>
    /// <param name="item">The to-do item to be transferred.</param>
    /// <param name="sourceList">The source to-do list.</param>
    /// <param name="targetList">The target to-do list.</param>
    /// <returns>True if the transfer was successful; otherwise, false.</returns>
    public bool TransferToDoItem(Account account, ToDoItem item, ToDoList sourceList, ToDoList targetList)
    {
        if (!sourceList.BelongsToAccount(account) || !targetList.BelongsToAccount(account))
            return false;

        if (targetList.GetAllItems().Any(i => i.Id == item.Id))
            return false;

        var removed = sourceList.DeleteItem(item.Id);
        if (!removed)
            return false;

        targetList.AddItem(item);

        return true;
    }
}
