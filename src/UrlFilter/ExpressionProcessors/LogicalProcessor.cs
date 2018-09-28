using System.Collections.Generic;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter.ExpressionTypes
{
    public class LogicalProcessor : IExpressionProcessor
    {
        private List<string> _operands;
        private ExpressionOperator _operators;
        public LogicalProcessor(List<string> operands, ExpressionOperator operators)
        {
            _operands = operands;
            _operators = operators;
        }

        public ExpressionCategory ExpressionCategory => ExpressionCategory.Logical;

        public bool CanProcess(string operand)
        {
            if (string.IsNullOrWhiteSpace(operand)) return false;
            return _operands.Contains(operand.ToLower());
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var current = tokens.First.Next;
            while (current != null && current.Next != null)
            {
                if (CanProcess(current.Value.TokenValue))
                {
                    var tokenValue = current.Value.TokenValue;
                    var leftExpression = current.Previous.Value.OperatorExpression;
                    var rightExpression = current.Next.Value.OperatorExpression;
                    var resultingExpression = _operators.OperatorExpression(tokenValue, leftExpression, rightExpression);

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
