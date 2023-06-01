using Dawn;
using System;
using System.Collections.Generic;
using Vita.Common;
using Vita.Core.Domain;
using Vita.Goals.Domain.Aggregates.Tasks;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Aggregates.Goals;

public class Goal : Entity
{
    private string _title;

    public GoalStatus GoalStatus { get; private set; }
    public string? Description { get; set; }
    public DateTimeInterval? AimDate { get; set; }
    public ICollection<Task> Tasks { get; set; }
    public Guid CreatedBy { get; init; }
    public DateTimeOffset CreatedOn { get; init; }

    protected Goal()
    {

    }

    public Goal(string title, Guid createdBy, string? description = null, DateTimeInterval? aimDate = null) : this()
    {
        Id = Guid.NewGuid();
        Title = title;
        CreatedBy = Guard.Argument(createdBy, nameof(CreatedBy)).NotDefault();
        Description = description;
        AimDate = aimDate;
        CreatedOn = DateTimeOffset.UtcNow;
        GoalStatus = GoalStatus.ToBeDefined;
    }

    public string Title
    {
        get => _title;
        set => _title = Guard.Argument(value, nameof(Title)).NotNull().NotEmpty();
    }

    public void Complete()
    {
        GoalStatus = GoalStatus.Completed;
    }

    public void Ready()
    {
        GoalStatus = GoalStatus.Ready;
    }

    public void InProgress()
    {
        GoalStatus = GoalStatus.InProgress;
    }
}