using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraphQL_Linq
{
    /// <inheritdoc />
    public class GraphQLQueryExecutor : IGraphQLQueryExecutor
    {
        private readonly string _url;
        private readonly HttpClient _client = new HttpClient();

        public GraphQLQueryExecutor(string url)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <inheritdoc />
        public async Task<GraphQLDataResult<T>> ExecuteGraphQlDataResult<T>(string query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

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