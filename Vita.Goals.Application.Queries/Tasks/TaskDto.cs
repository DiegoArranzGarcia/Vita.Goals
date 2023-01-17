using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vita.Goals.Application.Queries.Tasks
{
    public record TaskDto
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset? PlannedDateStart { get; set; }
        public DateTimeOffset? PlannedDateEnd { get; set; }
        public string Status { get; set; }
    }
}
