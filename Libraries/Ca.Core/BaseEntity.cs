using System;

namespace Ca.Core
{
    /// <summary>
    /// Represents the base class for entities
    /// </summary>
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}