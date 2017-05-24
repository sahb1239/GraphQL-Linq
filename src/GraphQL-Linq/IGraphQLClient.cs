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
        /// Used to query the GraphQL client using Linq
        /// </summary>
        /// <typeparam name="T">Type to query against</typeparam>
        /// <returns>A <see cref="IQueryable{T}"/> with the type parameter T</returns>
        IQueryable<T> Query<T>() where T : class;
    }
}