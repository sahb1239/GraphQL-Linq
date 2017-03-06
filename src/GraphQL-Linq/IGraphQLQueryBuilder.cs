namespace GraphQL_Linq
{
    /// <summary>
    /// Builds a GraphQL query from a GraphQLQueryOptions object
    /// </summary>
    public interface IGraphQLQueryBuilder
    {
        /// <summary>
        /// Builds the query from the <see cref="GraphQLQueryOptions"/> object
        /// </summary>
        /// <param name="options">The options to build the query from</param>
        /// <returns>The generated GraphQL query</returns>
        string GetQuery(GraphQLQueryOptions options);
    }
}