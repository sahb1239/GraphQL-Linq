using System.Collections.Generic;
using System.Linq;

namespace GraphQL_Linq
{
    /// <summary>
    /// Result from a GraphQL query
    /// </summary>
    /// <typeparam name="T">The type of the result</typeparam>
    public class GraphQLDataResult<T>
    {
        /// <summary>
        /// The data which is returned from the GraphQL server of the type T
        /// </summary>
        public virtual T Data { get; set; }

        /// <summary>
        /// The errors which are returned from the GraphQL server
        /// </summary>
        public virtual IEnumerable<GraphQLError> Errors { get; set; }

        /// <summary>
        /// Returns true if the request was successfully by checking if the response does not contains any <see cref="Errors"/>
        /// </summary>
        public virtual bool Succeeded => !Errors.Any();
    }

    /// <summary>
    /// An error returned from a GraphQL response
    /// </summary>
    public class GraphQLError
    {
        /// <summary>
        /// The error message from the GraphQL server
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// The location/locations the error has happened
        /// </summary>
        public virtual IEnumerable<GraphQLLocation> Locations { get; set; }
    }

    /// <summary>
    /// The location of an error in a GraphQL response
    /// </summary>
    public class GraphQLLocation
    {
        /// <summary>
        /// The line in which the error has happened
        /// </summary>
        public virtual int Line { get; set; }

        /// <summary>
        /// The column in which the error has happened
        /// </summary>
        public virtual int Column { get; set; }
    }
}