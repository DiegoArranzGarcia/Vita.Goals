using Dawn;
using System;
using Vita.Common;
using Vita.Core.Domain;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Aggregates.Tasks;

public class Task : Entity
{
    private string _title;

    private int _taskStatusId;
    public virtual TaskStatus TaskStatus { get; private set; }
    public DateTimeInterval? PlannedDate { get; set; }
    public DateTimeOffset CreatedOn { get; init; }
    public virtual Goal AssociatedTo { get; set; }

    protected Task()
    {

    }

    public Task(string title, Goal associatedTo, DateTimeInterval? plannedDate = null) : this()
    {
        Id = Guid.NewGuid();
        Title = title;
        PlannedDate = plannedDate;
        AssociatedTo = associatedTo;
        CreatedOn = DateTimeOffset.UtcNow;
        _taskStatusId = TaskStatus.Ready.Id;
    }

    public string Title
    {
        get => _title;
        set => _title = Guard.Argument(value, nameof(Title)).NotNull().NotEmpty();
    }
}
