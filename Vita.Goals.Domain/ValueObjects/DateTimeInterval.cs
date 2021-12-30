using Dawn;
using System;
using System.Collections.Generic;
using Vita.Core.Domain;

namespace Vita.Goals.Domain.ValueObjects
{
    public class DateTimeInterval : ValueObject
    {
        public DateTimeOffset Start { get; init; }
        public DateTimeOffset End { get; init; }

        public DateTimeInterval(DateTimeOffset start, DateTimeOffset end)
        {
            Guard.Argument(end, nameof(end)).InRange(start, DateTime.MaxValue);

            Start = start;
            End = end;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }
    }
}
