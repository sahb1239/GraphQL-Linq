using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GraphQL_Linq.Tests
{
    public class GraphQLQueryableTests
    {
        private readonly GraphQLClientMock _client = new GraphQLClientMock();
        private readonly IQueryable<TestType> _queryable;

        public GraphQLQueryableTests()
        {
            _queryable = new GraphQLQueryable<TestType>(_client, new GraphQLQueryBuilder());
        }

        private class GraphQLClientMock : IGraphQLClient
        {
            public string Query { get; private set; }

            public Task<GraphQLDataResult<T>> ExecuteGraphQlDataResult<T>(string query) where T : class
            {
                Query = query;
                return Task.FromResult(new GraphQLDataResult<T>());
            }

            IQueryable<T> IGraphQLClient.Query<T>()
            {
                throw new NotImplementedException();
            }
        }

        private class TestType
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
        }

        private class MapTestType
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        [Fact]
        public void TestSelectAll()
        {
            _queryable.ToList();

            Assert.Equal("{\"query\":\"{TestType{A B C}}\"}", _client.Query);
        }

        [Fact]
        public void TestMapTypeSelectAll()
        {
            _queryable.Select(e => new {A = e.A, B = e.B}).ToList();

            Assert.Equal("{\"query\":\"{TestType{A B}}\"}", _client.Query);
        }
    }
}
