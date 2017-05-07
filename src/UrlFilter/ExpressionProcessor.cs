using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter
{
    internal class ExpressionProcessor
    {
        private readonly ExpressionOperator _operators;
        private List<string> _operands;

        public bool canProcess(string operand)
        {
            if (string.IsNullOrWhiteSpace(operand)) return false;
            return _operands.Contains(operand.ToLower());
        }

        public ExpressionProcessor(List<string> operands, ExpressionOperator operators)
        {
            _operands = operands;
            _operators = operators;
        }
        public Expression Process(ParameterExpression paramExpression, Expression left, string operation, Expression right)
        {
            if(canProcess(operation))
            {
                return _operators.OperatorExpression(operation, left, right);
            }

            return Expression.Empty();
        }
    }
}
