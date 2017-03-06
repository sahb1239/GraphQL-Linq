using System.Threading.Tasks;

namespace GraphQL_Linq
{
    /// <summary>
    /// Client to send queries to the GraphQL server
    /// </summary>
    public interface IGraphQLClient
    {
        /// <summary>
        /// Sends a GraphQL query to the server and returns a <see cref="GraphQLDataResult{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the returned data</typeparam>
        /// <param name="query">The query to send to the server</param>
        /// <returns>Returns the result of the GraphQL query</returns>
        Task<GraphQLDataResult<T>> ExecuteGraphQlDataResult<T>(string query) where T : class;
    }
}