using System;
using System.Collections.Generic;
using System.Linq;
using Vita.Core.Domain;

namespace Vita.Goals.Domain.Aggregates.Tasks
{
    public class TaskStatus : Enumeration
    {
        public static TaskStatus Ready => new(1, nameof(Ready));
        public static TaskStatus InProgress => new(2, nameof(InProgress));
        public static TaskStatus Completed => new(3, nameof(Completed));

        public TaskStatus(int id, string name) : base(id, name)
        {

        }

        public static IEnumerable<TaskStatus> GetAllValues() => new[] { Ready, InProgress, Completed };

        public static TaskStatus FromName(string name)
        {
            var state = GetAllValues().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
                throw new ArgumentException($"Possible values for TaskStatus: {string.Join(",", GetAllValues().Select(s => s.Name))}");

            return state;
        }

        public static TaskStatus From(int id)
        {
            var state = GetAllValues().SingleOrDefault(s => s.Id == id);

            if (state == null)
                throw new ArgumentException($"Possible values for TaskStatus: {string.Join(",", GetAllValues().Select(s => s.Name))}");

            return state;
        }
    }
}
