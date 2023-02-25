using System;
using System.Collections.Generic;
using System.Linq;
using Vita.Core.Domain;

namespace Vita.Goals.Domain.Aggregates.Goals;

public class GoalStatus : Enumeration
{
    public static GoalStatus ToBeDefined => new(1, nameof(ToBeDefined));
    public static GoalStatus Ready => new(2, nameof(Ready));
    public static GoalStatus InProgress => new(3, nameof(InProgress));
    public static GoalStatus Completed => new(4, nameof(Completed));

    public GoalStatus(int id, string name) : base(id, name)
    {

    }

    public static IEnumerable<GoalStatus> GetAllValues() => new[] { ToBeDefined, Ready, InProgress, Completed };

    public static GoalStatus FromName(string name)
    {
        var state = GetAllValues().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state == null)
            throw new ArgumentException($"Possible values for GoalStatus: {string.Join(",", GetAllValues().Select(s => s.Name))}");

        return state;
    }

    public static GoalStatus From(int id)
    {
        var state = GetAllValues().SingleOrDefault(s => s.Id == id);

        if (state == null)
            throw new ArgumentException($"Possible values for GoalStatus: {string.Join(",", GetAllValues().Select(s => s.Name))}");

        return state;
    }
}
