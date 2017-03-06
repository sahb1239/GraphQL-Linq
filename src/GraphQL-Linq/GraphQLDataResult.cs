namespace GraphQL_Linq
{
    /// <summary>
    /// Result from a GraphQL query
    /// </summary>
    /// <typeparam name="T">The type of the result</typeparam>
    public class GraphQLDataResult<T> where T : class
    {
        /// <summary>
        /// The data which is returned from the GraphQL server of the type <see cref="T"/>
        /// </summary>
        public T Data { get; set; }
    }
}