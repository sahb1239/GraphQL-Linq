using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace GraphQL_Linq
{
    /// <summary>
    /// GraphQLQueryable used to query data from a GraphQL server
    /// </summary>
    /// <typeparam name="T">The type of the input/result of the query</typeparam>
    public class GraphQLQueryable<T> : QueryableBase<T>
    {
        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        /// <inheritdoc />
        public GraphQLQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        /// <summary>
        /// Initilizes the GraphQL queryable from a GraphQL server url
        /// </summary>
        /// <param name="url">URL to the GraphQL endpoint (the endpoint should accept POST methods)</param>
        public GraphQLQueryable(string url) : this(new GraphQLClient(url), new GraphQLQueryBuilder()) { }

        /// <summary>
        /// Initilized the GraphQL queryable using a custom <see cref="IGraphQLClient"/> and a custom <see cref="IGraphQLQueryBuilder"/>
        /// </summary>
        /// <param name="client">The custom GraphQL client</param>
        /// <param name="queryBuilder">The custom GraphQL query builder</param>
        public GraphQLQueryable(IGraphQLClient client, IGraphQLQueryBuilder queryBuilder) : base(QueryParser.CreateDefault(), new GraphQLQueryExecutor(client, queryBuilder))
        {
            
        }
    }
}
