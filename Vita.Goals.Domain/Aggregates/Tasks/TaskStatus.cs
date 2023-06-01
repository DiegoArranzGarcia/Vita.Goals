using System;
using System.Collections.Generic;
using System.Linq;
using Vita.Core.Domain;

namespace Vita.Goals.Domain.Aggregates.Tasks;

public class TaskStatus : Enumeration
{
    public readonly static TaskStatus Ready = new(1, nameof(Ready));
    public readonly static TaskStatus InProgress = new(2, nameof(InProgress));
    public readonly static TaskStatus Completed = new(3, nameof(Completed));

    private TaskStatus(int id, string name) : base(id, name)
    {

    }    
}
