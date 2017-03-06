namespace GraphQL_Linq
{
    /// <summary>
    /// Defines the different options in which this GraphQL client offers
    /// </summary>
    public class GraphQLQueryOptions
    {
        /// <summary>
        /// Where the query should query from
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Defines what should be selected from the from clause
        /// </summary>
        public string Select { get; set; }
        
        /// <summary>
        /// Defines how many items which should be skipped
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Defines how many items which should be taken
        /// </summary>
        public int? Take { get; set; }
        
        /// <summary>
        /// Defines if the result should return a count 
        /// </summary>
        public bool ReturnCount { get; set; } = false;
    }
}