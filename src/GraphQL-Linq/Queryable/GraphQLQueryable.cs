using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;

namespace GraphQL_Linq.Queryable
{
    /// <summary>
    /// GraphQLQueryable used to query data from a GraphQL server
    /// </summary>
    /// <typeparam name="T">The type of the input/result of the query</typeparam>
    public class GraphQLQueryable<T> : QueryableBase<T>, IAsyncEnumerable<T>
    {
        public GraphQLQueryable(IGraphQLQueryExecutor queryExecutor)
            : this(queryExecutor, new GraphQLQueryBuilder())
        {

        }

        public GraphQLQueryable(IGraphQLQueryExecutor queryExecutor, IGraphQLQueryBuilder queryBuilder)
            : this(new GraphQLQueryProvider(new GraphQlQueryCompiler(queryExecutor, queryBuilder)))
        {
            
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        /// <inheritdoc />
        public GraphQLQueryable(IQueryProvider provider)
            : base(provider)
        {
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        /// <inheritdoc />
        public GraphQLQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        /// <inheritdoc />
        internal GraphQLQueryable(IAsyncQueryProvider provider)
            : base(provider)
        {
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        /// <inheritdoc />
        internal GraphQLQueryable(IAsyncQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator() 
            => ((IAsyncQueryProvider) Provider).ExecuteAsync<T>(Expression).GetEnumerator();
    }
}
