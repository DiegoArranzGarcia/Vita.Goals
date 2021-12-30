using Dawn;
using System;
using Vita.Core.Domain.Repositories;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Aggregates.Categories
{
    public class Category : Entity, IAggregateRoot
    {
        public Guid CreatedBy { get; init; }
        private HexColor _color;
        private string _name;

        public Category(string name, HexColor color, Guid createdBy)
        {
            Id = Guid.NewGuid();
            Name = name;
            Color = color;
            CreatedBy = Guard.Argument(createdBy, nameof(CreatedBy)).NotDefault();
        }

        public string Name
        {
            get => _name;
            set => _name = Guard.Argument(value, nameof(Name)).NotNull().NotEmpty();
        }

        public HexColor Color
        {
            get => _color;
            set => _color = Guard.Argument(value, nameof(Color)).NotNull();
        }
    }
}
