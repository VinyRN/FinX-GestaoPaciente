namespace FinX.Tests
{
    public class FakeHttpClientFactory : System.Net.Http.IHttpClientFactory
    {
        public System.Net.Http.HttpClient CreateClient(string name) => new System.Net.Http.HttpClient();
    }

}
