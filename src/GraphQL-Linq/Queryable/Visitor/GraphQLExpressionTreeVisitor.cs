using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System.Linq;

namespace GraphQL_Linq
{
    internal class GraphQLExpressionTreeVisitor : ThrowingExpressionVisitor
    {
        public static string GetGraphQLExpression(Expression linqExpression)
        {
            var visitor = new GraphQLExpressionTreeVisitor();
            visitor.Visit(linqExpression);
            return visitor.GetGraphQLExpression();
        }

        private readonly StringBuilder _expression = new StringBuilder();

        public string GetGraphQLExpression()
        {
            return _expression.ToString();
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            _expression.Append("{");

            _expression.Append(string.Join(" ", expression.Type.GetRuntimeProperties().Select(e => e.Name)));

            _expression.Append("}");
            
            return expression;
        }

        protected override Expression VisitNew(NewExpression expression)
        {
            _expression.Append("{");
            
            //var i = 0;
            _expression.Append(string.Join(" ", expression.Members.Select(e => e.Name)));
            /*
            foreach (var arg in expression.Arguments)
            {
                if (i != 0)
                    _expression.Append(" ");

                _expression.AppendFormat("{0} ", expression.Members[i].Name);
                //Visit(arg);
                i++;
            }*/
            _expression.Append("}");


            return expression;
        }

        // Called when a LINQ expression type is not handled above.
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        private static string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return unhandledItem.ToString();
            //return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }
    }
}