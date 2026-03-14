using FluentValidation.Results;

namespace ERP.GC.Presentation.Models
{
    public abstract class Entity
    {
        public int Id { get; protected set; }
        public ValidationResult ValidationResult { get; protected set; }


        public abstract bool IsValid();
    }
}
