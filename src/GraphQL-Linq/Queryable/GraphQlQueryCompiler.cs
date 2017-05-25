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
        private readonly IGraphQLQueryExecutor _queryExecutor;
        private readonly IGraphQLQueryBuilder _queryBuilder;
        private readonly IQueryParser _queryParser = QueryParser.CreateDefault();

        public GraphQlQueryCompiler(IGraphQLQueryExecutor queryExecutor, IGraphQLQueryBuilder queryBuilder)
        {
            _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
            _queryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
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
                _queryExecutor.ExecuteQuery<IDictionary<string, IEnumerable<T>>>(
                        JsonConvert.SerializeObject(new {query = _queryBuilder.GetQuery(visitor.GetGraphQLQueryOptions())}))
                    .GetAwaiter()
                    .GetResult();
            return result?.Data?.Values?.SelectMany(enumerable => enumerable) ?? Enumerable.Empty<T>();
        }

        public async Task<IEnumerable<T>> ExecuteCollectionAsync<T>(Expression query, CancellationToken token)
        {
            // Get QueryModel
            QueryModel queryModel = GetParsedQuery(query);

            GraphQLQueryVisitor visitor = new GraphQLQueryVisitor();
            visitor.VisitQueryModel(queryModel);

            GraphQLDataResult<IDictionary<string, IEnumerable<T>>> result =
                await _queryExecutor.ExecuteQuery<IDictionary<string, IEnumerable<T>>>(
                        JsonConvert.SerializeObject(new { query = _queryBuilder.GetQuery(visitor.GetGraphQLQueryOptions()) }));
            return result?.Data?.Values?.SelectMany(enumerable => enumerable) ?? Enumerable.Empty<T>();
        }

        private static readonly MethodInfo ExecuteCollectionMethodAsync = (typeof(GraphQlQueryCompiler).GetRuntimeMethod("ExecuteCollectionAsync", new[] { typeof(Expression), typeof(CancellationToken) }));

        /// <summary>
        /// Gets type parameter from a <see cref="IEnumerable{T}"/> type <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="IEnumerable{T}"/> type</typeparam>
        /// <returns>Returns the typeparameter T from the <see cref="IEnumerable{T}"/></returns>
        private Type GetIEnumerableType<T>()
        {
            var typeInfo = typeof(T);

            // Check if the type is a array
            if (typeInfo.IsArray)
            {
                return typeInfo.GetElementType();
            }

            // Check if the type is a IEnumerable<>
            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return typeInfo.GenericTypeArguments.First();
            }

            // Check if the type is a IAsyncEnumerable<>
            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
            {
                return typeInfo.GenericTypeArguments.First();
            }

            // Get the first implemented interface which is the type IEnumerable<>
            var interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIEnumerable);

            if (interfacesImplemented != null)
            {
                return interfacesImplemented.GenericTypeArguments.First();
            }

            // Get the first implemented interface which is the type IAsyncEnumerable<>
            interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIAsyncEnumerable);

            if (interfacesImplemented != null)
            {
                return interfacesImplemented.GenericTypeArguments.First();
            }

            throw new NotSupportedException($"The type {typeof(T).FullName} is not supported. It should be a IEnumerable<T> type");
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
            return await (Task<TResult>) executeMethod.Invoke(this, new object[] { query, cancellationToken });
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new AsyncEnumerableTask<TResult>(ExecuteAsync<IEnumerable<TResult>>(expression, CancellationToken.None));
        }
    }
}