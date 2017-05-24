using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL_Linq.Queryable;
using Xunit;

namespace GraphQL_Linq.Tests
{
    public class GraphQLQueryableTests
    {
        private readonly GraphQLClientMock _client = new GraphQLClientMock();
        private readonly IQueryable<TestType> _queryable;

        public GraphQLQueryableTests()
        {
            _queryable = _client.Query<TestType>();
        }

        private class GraphQLClientMock : IGraphQLClient
        {
            public string ExecutedQuery { get; private set; }

            public Task<GraphQLDataResult<T>> ExecuteGraphQlDataResult<T>(string query)
            {
                ExecutedQuery = query;
                return Task.FromResult(new GraphQLDataResult<T>());
            }

            public IQueryable<T> Query<T>() where T : class
            {
                return new GraphQLQueryable<T>(this);
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

            Assert.Equal("{\"query\":\"{TestType{A B C}}\"}", _client.ExecutedQuery);
        }

        [Fact]
        public async Task TestSelectAllAsync()
        {
            await _queryable.ToListAsync();

            Assert.Equal("{\"query\":\"{TestType{A B C}}\"}", _client.ExecutedQuery);
        }

        [Fact]
        public void TestMapTypeSelectAll()
        {
            _queryable.Select(e => new {A = e.A, B = e.B}).ToList();

            Assert.Equal("{\"query\":\"{TestType{A B}}\"}", _client.ExecutedQuery);
        }

        [Fact]
        public async Task TestMapTypeSelectAllAsync()
        {
            await _queryable.Select(e => new { A = e.A, B = e.B }).ToListAsync();

            Assert.Equal("{\"query\":\"{TestType{A B}}\"}", _client.ExecutedQuery);
        }
    }
}
