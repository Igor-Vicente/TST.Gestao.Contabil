namespace ERP.Registro.Presentation.Abstractions
{
    public interface INotificador
    {
        void AdicionarNotificacao(string notificacao);
        void AdicionarNotificacao(IEnumerable<string> notificacoes);
        bool TemNotificacoes();
        IEnumerable<string> ObterNotificacoes();
    }

    public class Notificador : INotificador
    {
        private readonly List<string> _notificacoes;

        public Notificador()
        {
            _notificacoes = new();
        }

        public void AdicionarNotificacao(string notificacao)
        {
            _notificacoes.Add(notificacao);
        }
        public void AdicionarNotificacao(IEnumerable<string> notificacoes)
        {
            _notificacoes.AddRange(notificacoes);
        }

        public bool TemNotificacoes() => _notificacoes.Any();

        public IEnumerable<string> ObterNotificacoes() => _notificacoes;
    }
}
