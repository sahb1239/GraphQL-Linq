using System;
using System.Linq;
using GraphQL_Linq.Queryable;

namespace GraphQL_Linq
{
    /// <inheritdoc />
    public class GraphQLClient : IGraphQLClient
    {
        private readonly IAsyncQueryProvider _queryProvider;

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified GraphQL server url using a default builder <see cref="GraphQLQueryBuilder"/>
        /// </summary>
        /// <param name="url">URL to the GraphQL endpoint (the endpoint should accept POST methods)</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphQLClient(string url) : this(url, new GraphQLQueryBuilder())
        {
        }

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified GraphQL server url using a default QueryExecutor <see cref="GraphQLQueryExecutor"/>
        /// </summary>
        /// <param name="url">URL to the GraphQL endpoint (the endpoint should accept POST methods)</param>
        /// <param name="queryBuilder">The query builder used to generate the query</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphQLClient(string url, IGraphQLQueryBuilder queryBuilder) : this(queryBuilder, new GraphQLQueryExecutor(url))
        {
        }

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified GraphQL query executor
        /// </summary>
        /// <param name="queryBuilder">The query builder used to generate the query</param>
        /// <param name="queryExecutor">The queryExecutor used to execute the GraphQL query</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphQLClient(IGraphQLQueryBuilder queryBuilder, IGraphQLQueryExecutor queryExecutor)
            : this(new GraphQlQueryCompiler(queryExecutor, queryBuilder))
        {
        }

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified <see cref="IQueryCompiler"/>
        /// </summary>
        /// <param name="queryCompiler">The QueryCompiler used to compile the queries</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal GraphQLClient(IQueryCompiler queryCompiler)
            : this(new GraphQLQueryProvider(queryCompiler))
        {
        }

        /// <summary>
        /// Initilizes a GraphQL client which communicates with the specified <see cref="IQueryCompiler"/>
        /// </summary>
        /// <param name="queryProvider">The QueryProvider used to execute the queries</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal GraphQLClient(IAsyncQueryProvider queryProvider)
        {
            _queryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
        }

        /// <inheritdoc />
        public IQueryable<T> Query<T>() where T : class
        {
            return new GraphQLQueryable<T>(_queryProvider);
        }
    }
}