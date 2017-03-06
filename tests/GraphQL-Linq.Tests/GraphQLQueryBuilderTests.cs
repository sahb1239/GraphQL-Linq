using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GraphQL_Linq.Tests
{
    public class GraphQLQueryBuilderTests
    {
        private readonly GraphQLQueryBuilder _builder = new GraphQLQueryBuilder();

        [Fact]
        public void SelectQuery()
        {
            GraphQLQueryOptions options = new GraphQLQueryOptions
            {
                From = "human",
                Select = "{id name}"
            };

            var query = _builder.GetQuery(options);

            Assert.Equal("{human{id name}}", query);
        }

        [Fact]
        public void ArgumentSkipQuery()
        {
            GraphQLQueryOptions options = new GraphQLQueryOptions
            {
                From = "human",
                Select = "{id name}",
                Skip = 5
            };
            var query = _builder.GetQuery(options);

            Assert.Equal("{human(skip:5){id name}}", query);
        }

        [Fact]
        public void ArgumentTakeQuery()
        {
            GraphQLQueryOptions options = new GraphQLQueryOptions
            {
                From = "human",
                Select = "{id name}",
                Take = 5
            };
            var query = _builder.GetQuery(options);

            Assert.Equal("{human(take:5){id name}}", query);
        }

        [Fact]
        public void ArgumentSkipTakeQuery()
        {
            GraphQLQueryOptions options = new GraphQLQueryOptions
            {
                From = "human",
                Select = "{id name}",
                Skip = 2,
                Take = 5
            };
            var query = _builder.GetQuery(options);

            Assert.Equal("{human(skip:2 take:5){id name}}", query);
        }

        [Fact]
        public void ArgumentCountQuery()
        {
            GraphQLQueryOptions options = new GraphQLQueryOptions
            {
                From = "human",
                Select = "{id name}",
                ReturnCount = true
            };
            var query = _builder.GetQuery(options);

            Assert.Equal("{humanCount{count}}", query);
        }
    }
}
