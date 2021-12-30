using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Categories
{
    public record CreateCategoryCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public Guid CreatedBy { get; set; }
    }
}