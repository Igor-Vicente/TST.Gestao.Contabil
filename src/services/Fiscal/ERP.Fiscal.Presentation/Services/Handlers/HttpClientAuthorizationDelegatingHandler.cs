using Shared.Identidade;

namespace ERP.Fiscal.Presentation.Services.Handlers
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IAuthUser _authtUser;

        public HttpClientAuthorizationDelegatingHandler(IAuthUser authUser)
        {
            _authtUser = authUser;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _authtUser.ObterHttpContext().Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string> { authorizationHeader });
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
