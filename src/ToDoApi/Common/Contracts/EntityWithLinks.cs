using System.Collections.Generic;

namespace ToDoApi.Common.Contracts;

/// <summary>
/// Wrapper for an entity with associated HATEOAS links.
/// </summary>
/// <typeparam name="T">Type of the entity.</typeparam>
public class EntityWithLinks<T>
{
    /// <summary>
    /// The entity data.
    /// </summary>
    public T? Entity { get; set; }

    /// <summary>
    /// List of HATEOAS links related to the entity.
    /// </summary>
    public List<Link> Links { get; set; } = new List<Link>();
}
