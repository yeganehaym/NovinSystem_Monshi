using RestSharp;

namespace WebApplication2.RestSharp;

public static class SmsClient
{
    private static RestClient _client;

    public static RestClient GetClient()
    {
        if (_client == null)
        {
            _client = new RestClient("https://api.ghasedak.me/v2");
        }

        return _client;
    }

    public static RestRequest GetRequest(string url)
    {
        var request = new RestRequest(url);
        request.AddHeader("apikey", "5261bd912e2a475e456b24eac7854a6228d934c386e2e0e900d90bbf83585310");
        return request;
    }
}