using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GraphQL_Linq.Queryable;
using Newtonsoft.Json;

namespace GraphQL_Linq
{
    /// <inheritdoc />
    public class GraphQLClient : IGraphQLClient, IGraphQLQueryExecutor
    {
        private readonly string _url;
        private readonly HttpClient _client = new HttpClient();
        private readonly IAsyncQueryProvider _queryProvider;

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified GraphQL server url using <see cref="GraphQLQueryBuilder"/>
        /// </summary>
        /// <param name="url">URL to the GraphQL endpoint (the endpoint should accept POST methods)</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphQLClient(string url) : this(url, new GraphQLQueryBuilder())
        {
        }

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified GraphQL server url
        /// </summary>
        /// <param name="url">URL to the GraphQL endpoint (the endpoint should accept POST methods)</param>
        /// <param name="queryBuilder">The query builder used to query the GraphQL server</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphQLClient(string url, GraphQLQueryBuilder queryBuilder)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (queryBuilder == null) throw new ArgumentNullException(nameof(queryBuilder));

            _url = url;
            _queryProvider = new GraphQLQueryProvider(new GraphQlQueryCompiler(this, queryBuilder));
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

        /// <inheritdoc />
        public IQueryable<T> Query<T>() where T : class
        {
            return new GraphQLQueryable<T>(_queryProvider);
        }
    }
}