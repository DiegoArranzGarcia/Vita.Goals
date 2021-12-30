using System;

namespace Vita.Goals.Application.Queries.Categories
{
    public record CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}