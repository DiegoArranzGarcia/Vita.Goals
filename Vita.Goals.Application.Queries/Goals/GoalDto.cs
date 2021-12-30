﻿using System;

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
    }
}