using System;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace GraphQL_Linq
{
    internal class GraphQLQueryVisitor : QueryModelVisitorBase
    {
        public GraphQLQueryOptions GetGraphQLQueryOptions()
        {
            return _queryBuilder;
        }

        private GraphQLQueryOptions _queryBuilder = new GraphQLQueryOptions();

        public override void VisitQueryModel(QueryModel queryModel)
        {
            queryModel.SelectClause.Accept(this, queryModel);
            queryModel.MainFromClause.Accept(this, queryModel);
            VisitBodyClauses(queryModel.BodyClauses, queryModel);
            VisitResultOperators(queryModel.ResultOperators, queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (resultOperator is FirstResultOperator)
            {
                _queryBuilder.Take = 1;
                return;
            }
            if (resultOperator is CountResultOperator)
            {
                _queryBuilder.ReturnCount = true;
                return;
            }

            if (resultOperator is SkipResultOperator)
            {
                _queryBuilder.Skip = ((SkipResultOperator) resultOperator).GetConstantCount();
                return;
            }

            if (resultOperator is TakeResultOperator)
            {
                _queryBuilder.Take = ((TakeResultOperator)resultOperator).GetConstantCount();
                return;
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            _queryBuilder.From = fromClause.ItemType.Name;
            base.VisitMainFromClause(fromClause, queryModel);
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("Additional froms not supported");
            //_queryBuilder.FromParts.Add(fromClause.ItemType.Name);
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            _queryBuilder.Select = GraphQLExpressionTreeVisitor.GetGraphQLExpression(selectClause.Selector);
            base.VisitSelectClause(selectClause, queryModel);
        }
    }
}