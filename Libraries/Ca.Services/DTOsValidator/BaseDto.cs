using System;

namespace Ca.Services.DTOs
{
    public abstract class BaseDto
    {
        public BaseDto()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}