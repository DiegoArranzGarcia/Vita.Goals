using Vita.Core.Domain;

namespace Vita.Goals.Domain.Aggregates.Goals;

public sealed class GoalStatus : Enumeration
{
    public static readonly GoalStatus ToBeDefined = new(1, nameof(ToBeDefined));
    public static readonly GoalStatus Ready = new(2, nameof(Ready));
    public static readonly GoalStatus InProgress = new(3, nameof(InProgress));
    public static readonly GoalStatus Completed = new(4, nameof(Completed));

    private GoalStatus(int id, string name) : base(id, name)
    {
    }
}
