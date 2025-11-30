using System.Net.Http.Headers;

namespace CliniPlus.Movil.Services.Http
{
    public class TokenMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = await SecureStorage.GetAsync("jwt");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =

                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, ct);
        }
    }
}
