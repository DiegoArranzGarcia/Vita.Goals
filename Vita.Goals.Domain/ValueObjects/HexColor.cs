using Dawn;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vita.Core.Domain;

namespace Vita.Goals.Domain.ValueObjects
{
    public class HexColor : ValueObject
    {
        private const string HexColorPattern = @"^#([a-f0-9]){6}$";
        private const RegexOptions EmailRegexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private readonly static Regex HexColorRegex = new(HexColorPattern, EmailRegexOptions);

        public string Color { get; init; }

        public HexColor(string color)
        {
            Color = Guard.Argument(color, nameof(color)).NotNull().NotEmpty().Matches(HexColorRegex);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Color;
        }
    }
}
