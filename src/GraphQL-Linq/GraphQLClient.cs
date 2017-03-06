using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraphQL_Linq
{
    /// <inheritdoc />
    public class GraphQLClient : IGraphQLClient
    {
        private readonly string _url;
        private readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified GraphQL server url
        /// </summary>
        /// <param name="url">URL to the GraphQL endpoint (the endpoint should accept POST methods)</param>
        public GraphQLClient(string url)
        {
            _url = url;
        }

        /// <inheritdoc />
        public async Task<GraphQLDataResult<T>> ExecuteGraphQlDataResult<T>(string query) where T : class
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _url)
            {
                Content = new StringContent(query, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage result = await _client.SendAsync(request);
            result.EnsureSuccessStatusCode();

            string response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GraphQLDataResult<T>>(response);
        }
    }
}