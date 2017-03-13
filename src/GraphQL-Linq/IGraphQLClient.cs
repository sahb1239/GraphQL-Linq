using System;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

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
        /// <exception cref="ArgumentNullException"></exception>
        Task<GraphQLDataResult<T>> ExecuteGraphQlDataResult<T>(string query) where T : class;

        /// <summary>
        /// Used to query the GraphQL client using Linq
        /// </summary>
        /// <typeparam name="T">Type to query against</typeparam>
        /// <returns>A <see cref="IQueryable"/> with the type parameter <see cref="T"/></returns>
        IQueryable<T> Query<T>() where T : class;
    }
}