using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL_Linq.Queryable
{
    /// <inheritdoc />
    internal class GraphQLQueryProvider : IAsyncQueryProvider
    {
        private readonly IQueryCompiler _queryCompiler;

        private static readonly MethodInfo _genericCreateQueryMethod
            = typeof(GraphQLQueryProvider).GetRuntimeMethods()
                .Single(m => (m.Name == "CreateQuery") && m.IsGenericMethod);

        public GraphQLQueryProvider(IQueryCompiler queryCompiler)
        {
            _queryCompiler = queryCompiler ?? throw new ArgumentNullException(nameof(queryCompiler));
        }

        /// <inheritdoc />
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new GraphQLQueryable<TElement>(this, expression);
        }

        /// <inheritdoc />
        public object Execute(Expression expression)
        {
            return _queryCompiler.Execute<object>(expression);
        }

        /// <inheritdoc />
        public TResult Execute<TResult>(Expression expression)
        {
            return _queryCompiler.Execute<TResult>(expression);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return _queryCompiler.ExecuteAsync<TResult>(expression);
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return _queryCompiler.ExecuteAsync<TResult>(expression, cancellationToken);
        }
    }
}