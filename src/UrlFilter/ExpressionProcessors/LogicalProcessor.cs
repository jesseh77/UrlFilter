using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter.ExpressionProcessors
{
    public class LogicalProcessor : IProcessExpression
    {
        private readonly string operand;
        private Func<Expression, Expression, Expression> expression;
        public LogicalProcessor(string operand, Func<Expression, Expression, Expression> expression)
        {
            this.operand = operand;
            this.expression = expression;
        }

        public ExpressionCategory ExpressionCategory => ExpressionCategory.Logical;

        public bool CanProcess(string operand, ParameterExpression paramExpression)
        {
            if (string.IsNullOrWhiteSpace(operand)) return false;
            return this.operand.Equals(operand, StringComparison.CurrentCultureIgnoreCase);
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var current = tokens.First.Next;
            while (current != null && current.Next != null)
            {
                if (CanProcess(current.Value.TokenValue, paramExpression))
                {
                    var tokenValue = current.Value.TokenValue;
                    var leftExpression = current.Previous.Value.OperatorExpression;
                    var rightExpression = current.Next.Value.OperatorExpression;
                    var resultingExpression = expression(leftExpression, rightExpression);

                    current.Value.OperatorExpression = resultingExpression;
                    current.Value.TokenValue = string.Empty;
                    tokens.Remove(current.Previous);
                    tokens.Remove(current.Next);
                }

                current = current.Next;
            }
        }
    }
}
