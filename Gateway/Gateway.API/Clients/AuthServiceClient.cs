namespace Gateway.Api.Clients;

public class AuthServiceClient
{
    private readonly HttpClient _http;

    public AuthServiceClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<HttpResponseMessage> Login(object request)
    {
        return await _http.PostAsJsonAsync("/auth/login", request);
    }
}
