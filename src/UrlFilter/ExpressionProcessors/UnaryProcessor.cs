using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class UnaryProcessor : IProcessExpression
    {
        private string operand;
        private readonly Func<Expression, Expression> expression;

        public ExpressionCategory ExpressionCategory => ExpressionCategory.Unary;

        public bool CanProcess(string operand, ParameterExpression paramExpression)
        {
            if (string.IsNullOrWhiteSpace(operand)) return false;
            return this.operand.Equals(operand, StringComparison.CurrentCultureIgnoreCase);
        }

        public UnaryProcessor(string operand, Func<Expression, Expression> expression)
        {
            this.operand = operand;
            this.expression = expression;
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (CanProcess(current.Value.TokenValue, paramExpression))
                {
                    var nextExpression = current.Next.Value.OperatorExpression;
                    var processedExpression = expression(nextExpression);
                    current.Value.OperatorExpression = processedExpression;
                    tokens.Remove(current.Next);
                }
                else
                {
                    current = current.Next;
                }
            }
        }
    }
}
