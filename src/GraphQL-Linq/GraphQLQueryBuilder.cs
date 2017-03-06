using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQL_Linq
{
    /// <inheritdoc />
    public class GraphQLQueryBuilder : IGraphQLQueryBuilder
    {
        /// <inheritdoc />
        public string GetQuery(GraphQLQueryOptions options)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            if (options.ReturnCount)
            {
                builder.Append(options.From + "Count{count}");
            }
            else
            {
                builder.Append(options.From);

                List<string> arguments = new List<string>();
                if (options.Skip.HasValue)
                {
                    arguments.Add("skip:" + options.Skip.Value);
                }
                if (options.Take.HasValue)
                {
                    arguments.Add("take:" + options.Take.Value);
                }
                if (arguments.Any())
                {
                    builder.AppendFormat("({0})", string.Join(" ", arguments));
                }

                builder.Append(options.Select);
            }

            builder.Append("}");

            return builder.ToString();
        }
    }
}