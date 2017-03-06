using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Remotion.Linq;

namespace GraphQL_Linq
{
    internal class GraphQLQueryExecutor : IQueryExecutor
    {
        private readonly IGraphQLClient _client;
        private readonly IGraphQLQueryBuilder _queryBuilder;

        public GraphQLQueryExecutor(IGraphQLClient client, IGraphQLQueryBuilder queryBuilder)
        {
            _client = client;
            _queryBuilder = queryBuilder;
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).Single();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty
                ? ExecuteCollection<T>(queryModel).SingleOrDefault()
                : ExecuteCollection<T>(queryModel).Single();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            GraphQLQueryVisitor visitor = new GraphQLQueryVisitor();
            visitor.VisitQueryModel(queryModel);

            GraphQLDataResult<IDictionary<string, IEnumerable<T>>> result =
                _client.ExecuteGraphQlDataResult<IDictionary<string, IEnumerable<T>>>(
                        JsonConvert.SerializeObject(new {query = _queryBuilder.GetQuery(visitor.GetGraphQLQueryOptions())}))
                    .GetAwaiter()
                    .GetResult();
            return result?.Data?.Values?.SelectMany(enumerable => enumerable) ?? new List<T>();
        }
    }
}