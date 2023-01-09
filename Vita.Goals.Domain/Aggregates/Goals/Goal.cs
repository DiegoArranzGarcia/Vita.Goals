using Dawn;
using System;
using Vita.Core.Domain.Repositories;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Aggregates.Goals
{
    public class Goal : Entity
    {
        private string _title;
        private int _goalStatusId;
        public string Description { get; set; }
        public DateTimeInterval AimDate { get; set; }
        public Guid CreatedBy { get; init; }
        public DateTimeOffset CreatedOn { get; init; }

        protected Goal()
        {

        }

        public Goal(string title, Guid createdBy, string description = null, DateTimeInterval aimDate = null) : this()
        {
            Id = Guid.NewGuid();
            Title = title;
            CreatedBy = Guard.Argument(createdBy, nameof(CreatedBy)).NotDefault();
            Description = description;
            AimDate = aimDate;
            CreatedOn = DateTimeOffset.UtcNow;
            GoalStatus = GoalStatus.ToBeDefined;
        }

        public void Complete()
        {
            Guard.Argument(GoalStatus).Equal(GoalStatus.ToBeDefined);
            GoalStatus = GoalStatus.Completed;
        }

        public GoalStatus GoalStatus
        {
            get => GoalStatus.From(_goalStatusId);
            private set => _goalStatusId = value.Id;
        }

        public string Title
        {
            get => _title;
            set => _title = Guard.Argument(value, nameof(Title)).NotNull().NotEmpty();
        }
    }
}