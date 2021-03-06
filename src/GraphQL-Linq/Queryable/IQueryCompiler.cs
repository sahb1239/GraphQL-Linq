using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL_Linq.Queryable
{
    /// <summary>
    /// Support execution of a <see cref="Expression"/> from a <see cref="IQueryable{T}"/>
    /// </summary>
    internal interface IQueryCompiler
    {
        /// <summary>
        /// Executes the query represented by a specified expression tree
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query</typeparam>
        /// <param name="query">An expression tree that represents a LINQ query</param>
        /// <returns>The value that results from executing the specified query</returns>
        TResult Execute<TResult>(Expression query);

        /// <summary>
        /// Executes the query async represented by a specified expression tree
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query</typeparam>
        /// <param name="expression">An expression tree that represents a LINQ query</param>
        /// <returns>The value that results from executing the specified query</returns>
        IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression);

        /// <summary>
        /// Executes the query represented by a specified expression tree
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query</typeparam>
        /// <param name="query">An expression tree that represents a LINQ query</param>
        /// <param name="cancellationToken">The cancellationToken used to cancel the request</param>
        /// <returns>The value that results from executing the specified query</returns>
        Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken);
    }
}