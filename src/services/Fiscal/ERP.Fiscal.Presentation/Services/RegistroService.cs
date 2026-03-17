using ERP.Fiscal.Presentation.ViewModels;
using System.Text.Json;

namespace ERP.Fiscal.Presentation.Services
{
    public interface IRegistroService
    {
        Task<UsuarioRespostaViewModel> ObterUsuarioEmpresaAsync(int id);
    }

    public class RegistroService : IRegistroService
    {
        private readonly HttpClient _httpClient;

        public RegistroService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UsuarioRespostaViewModel> ObterUsuarioEmpresaAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/v1/registro/usuario/{id}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UsuarioRespostaViewModel>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
