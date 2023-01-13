using System;
using System.Collections.Generic;
using System.Linq;

namespace Vita.Goals.Application.Queries.Goals
{
    public record GoalDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? AimDateStart { get; set; }
        public DateTimeOffset? AimDateEnd { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
    }

    public record TaskDto
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset? PlannedDateStart { get; set; }
        public DateTimeOffset? PlannedDateEnd{ get; set; }
        public string Status { get; set; }
    }
}