using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GraphQL_Linq.Queryable.Visitor;
using Newtonsoft.Json;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace GraphQL_Linq.Queryable
{
    internal class GraphQlQueryCompiler : IQueryCompiler
    {
        private readonly IGraphQLClient _client;
        private readonly IGraphQLQueryBuilder _queryBuilder;
        private readonly IQueryParser _queryParser = QueryParser.CreateDefault();

        public GraphQlQueryCompiler(IGraphQLClient client, IGraphQLQueryBuilder queryBuilder)
        {
            _client = client;
            _queryBuilder = queryBuilder;
        }

        private QueryModel GetParsedQuery(Expression expression)
        {
            return _queryParser.GetParsedQuery(expression);
        }

        public IEnumerable<T> ExecuteCollection<T>(Expression query)
        {
            // Get QueryModel
            QueryModel queryModel = GetParsedQuery(query);

            GraphQLQueryVisitor visitor = new GraphQLQueryVisitor();
            visitor.VisitQueryModel(queryModel);

            GraphQLDataResult<IDictionary<string, IEnumerable<T>>> result =
                _client.ExecuteGraphQlDataResult<IDictionary<string, IEnumerable<T>>>(
                        JsonConvert.SerializeObject(new {query = _queryBuilder.GetQuery(visitor.GetGraphQLQueryOptions())}))
                    .GetAwaiter()
                    .GetResult();
            return result?.Data?.Values?.SelectMany(enumerable => enumerable) ?? Enumerable.Empty<T>();
        }

        public async Task<IEnumerable<T>> ExecuteCollectionAsync<T>(Expression query)
        {
            // Get QueryModel
            QueryModel queryModel = GetParsedQuery(query);

            GraphQLQueryVisitor visitor = new GraphQLQueryVisitor();
            visitor.VisitQueryModel(queryModel);

            GraphQLDataResult<IDictionary<string, IEnumerable<T>>> result =
                await _client.ExecuteGraphQlDataResult<IDictionary<string, IEnumerable<T>>>(
                        JsonConvert.SerializeObject(new { query = _queryBuilder.GetQuery(visitor.GetGraphQLQueryOptions()) }));
            return result?.Data?.Values?.SelectMany(enumerable => enumerable) ?? Enumerable.Empty<T>();
        }

        private static readonly MethodInfo ExecuteCollectionMethodAsync = (typeof(GraphQlQueryCompiler).GetRuntimeMethod("ExecuteCollectionAsync", new[] { typeof(Expression) }));

        private Type GetIEnumerableType<T>()
        {
            var typeInfo = typeof(T);

            if (typeInfo.IsArray)
            {
                return typeInfo.GetElementType();
            }

            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return typeInfo.GenericTypeArguments.First();
            }

            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
            {
                return typeInfo.GenericTypeArguments.First();
            }

            var interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIEnumerable);

            if (interfacesImplemented != null)
            {
                return interfacesImplemented.GenericTypeArguments.First();
            }

            interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIAsyncEnumerable);

            if (interfacesImplemented != null)
            {
                return interfacesImplemented.GenericTypeArguments.First();
            }

            throw new NotSupportedException();
        }

        private static bool IsIEnumerable(TypeInfo type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type);
        }

        private static bool IsGenericIEnumerable(TypeInfo enumerableType)
        {
            return IsIEnumerable(enumerableType)
                   && enumerableType.IsGenericType
                   && enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsGenericIAsyncEnumerable(TypeInfo enumerableType)
        {
            return enumerableType.IsGenericType
                   && enumerableType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
        }

        public TResult Execute<TResult>(Expression query)
            => ExecuteAsync<TResult>(query, CancellationToken.None).GetAwaiter().GetResult();

        public async Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken) 
        {
            // Execution method
            var executeMethod = ExecuteCollectionMethodAsync.MakeGenericMethod(GetIEnumerableType<TResult>());
            return await (Task<TResult>) executeMethod.Invoke(this, new[] { query });
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new AsyncEnumerableTask<TResult>(ExecuteAsync<IEnumerable<TResult>>(expression, CancellationToken.None));
        }
    }
}