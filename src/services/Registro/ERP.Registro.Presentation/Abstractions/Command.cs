using MediatR;

namespace ERP.Registro.Presentation.Abstractions
{
    public class Command<T> : IRequest<T>
    {
        public DateTime Timestamp { get; private set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }
    }
}
